using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.IO;

public class SettingsWindow : EditorWindow
{
    [Serializable]
    private class ApiInfo
    {
        public GameInfo[] games;
        public string module_version;

        public ApiInfo(GameInfo[] games, string module_version)
        {
            this.games = games;
            this.module_version = module_version;
        }
    }

    [Serializable]
    private class GameInfo
    {
        public int id;
        public string name;
        public string slug;
        public string version;
        public bool retired;
        public string[] environments;

        public GameInfo(int id, string name, string slug, string version, bool retired, string[] environments)
        {
            this.id = id;
            this.name = name;
            this.slug = slug;
            this.version = version;
            this.retired = retired;
            this.environments = environments;
        }
    }

    private static GUIStyle reloadStyle;

    private UnityWebRequest request;
    private DataSettings settings;
    private GameInfo[] games;
    private string latestModuleVersion;
    private string error;
    private bool enabled = false;

    [MenuItem("Modules/Data Manager/Settings", false, 21)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SettingsWindow));
    }

    void Awake()
    {
        titleContent = new GUIContent("Settings");
        settings = AssetDatabase.LoadAssetAtPath(DataSettings.FILE_DIR, typeof(DataSettings)) as DataSettings;

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<DataSettings>();
            settings.buildEnvironment = DataManager.Environment.Staging;

            if (!AssetDatabase.IsValidFolder(DataSettings.FOLDER))
                Directory.CreateDirectory(DataSettings.FOLDER);

            AssetDatabase.CreateAsset(settings, DataSettings.FILE_DIR);
            AssetDatabase.SaveAssets();
        }

        updateServerInfo();
    }

    void OnGUI()
    {
        string[] gameNames = getGameNames();
        int selectedGameIndex = settings.game != null ? ArrayUtility.IndexOf(gameNames, settings.game) : -1;

        if (gameNames.Length == 0 && settings.game != null)
            gameNames = new string[] { settings.game };

        setReloadStyle();

        GUI.enabled = enabled;

        GUILayout.BeginHorizontal();
        {
            selectedGameIndex = EditorGUILayout.Popup("Game:", selectedGameIndex, gameNames);

            if (GUILayout.Button("Reload", reloadStyle))
                updateServerInfo();
        }
        GUILayout.EndHorizontal();

        settings.buildEnvironment = (DataManager.Environment)EditorGUILayout.EnumPopup("Build mode:", settings.buildEnvironment);
        settings.offline = EditorGUILayout.Toggle("Offline mode:", settings.offline);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("Editor will default to run in Staging when build mode is set to Production", MessageType.Warning);

        GUILayout.Label("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Module version: ", DataManager.MODULE_VERSION);
        EditorGUILayout.LabelField("Lastest online version: ", latestModuleVersion != null && latestModuleVersion.Length > 0 ? latestModuleVersion : "-");

        if (error != null && error.Length > 0)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox(error, MessageType.Error);
        }

        if (GUI.changed)
        {
            if (selectedGameIndex >= 0 && settings.game != gameNames[selectedGameIndex])
            {
                settings.game = gameNames[selectedGameIndex];
                settings.gameId = getGameByName(settings.game).id;
            }
            EditorUtility.SetDirty(settings);
        }
    }

    void Update()
    {
        if (request != null && request.isDone)
        {
            latestModuleVersion = null;
            games = null;

            if (request.isNetworkError)
            {
                error = request.error;
            }
            else
            {
                try
                {
                    ServerResponse<ApiInfo> response = JsonUtility.FromJson<ServerResponse<ApiInfo>>(request.downloadHandler.text);

                    if (response.success)
                    {
                        ApiInfo apiInfo = response.data;

                        if (apiInfo.games.Length > 0)
                            games = apiInfo.games;

                        GameInfo game = getGameByName(settings.game);

                        if (game != null)
                        {
                            settings.gameId = game.id;
                            AssetDatabase.SaveAssets();
                        }

                        latestModuleVersion = apiInfo.module_version;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(request.downloadHandler.text);
                    error = e.Message;
                }
            }

            request = null;
            enabled = true;
        }
    }

    private void updateServerInfo()
    {
        if (request != null)
            return;

        request = UnityWebRequest.Post(DataManager.API_ROOT_URL_STAGING + "game_versions/v1/get_games", "");
        request.SetRequestHeader("api-key", DataManager.API_KEY);
        request.Send();
        enabled = false;
        error = null;
    }

    private string[] getGameNames()
    {
        if (games == null)
            return new string[] { };
        string[] names = new string[games.Length];
        for (int i = 0; i < games.Length; i++)
            names[i] = games[i].name != null && games[i].name.Length > 0 ? games[i].name : games[i].slug;
        return names;
    }

    private GameInfo getGameByName(string name)
    {
        if (name == null || name.Length == 0 || games == null)
            return null;

        foreach (GameInfo game in games)
            if (game.name == name || game.slug == name)
                return game;

        return null;
    }

    private void setReloadStyle()
    {
        if (reloadStyle != null)
            return;

        GUIStyle popup = EditorStyles.popup;
        reloadStyle = new GUIStyle(GUI.skin.button);
        reloadStyle.fontSize = popup.fontSize;
        reloadStyle.fixedHeight = popup.fixedHeight;
        reloadStyle.margin = popup.margin;
    }
}
