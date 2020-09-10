using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections.Generic;

public class DatabaseWindow : EditorWindow
{
    [Serializable]
    private class Field
    {
        public string name;
        public string type;
        public string desc;

        public Field(string name, string type, string desc)
        {
            this.name = name;
            this.type = type;
            this.desc = desc;
        }
    }

    [Serializable]
    private class Table
    {
        public string name;
        public DataManager.Service service;
        public Field[] fields;
        public bool expanded;

        public Table(string name, DataManager.Service service, Field[] fields)
        {
            this.name = name;
            this.service = service;
            this.fields = fields;
        }
    }

    [Serializable]
    private class TableInfo
    {
        public Table[] tables;

        public TableInfo(Table[] tables)
        {
            this.tables = tables;
        }
    }

    private static GUIStyle reloadStyle;
    private static GUIStyle serviceStyle;
    private static GUIStyle labelStyle;

    private UnityWebRequest request;
    private DataSettings settings;
    private DataManager.Environment environment;
    private Vector2 scroll;
    private Table[] tables;
    private string game;
    private string error;
    private bool expandAll = true;
    private bool enabled = false;

    [MenuItem("Modules/Data Manager/Data Objects", false, 22)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DatabaseWindow));
    }   

    void Awake()
    {
        titleContent = new GUIContent("Data Objects");
        environment = DataManager.Environment.Development;
    }

    void OnGUI()
    {
        loadSettings();

        if (settings == null || settings.game == null || settings.game.Length == 0)
        {
            EditorGUILayout.HelpBox("Please select a game from Modules > DataManager > Settings", MessageType.Warning);
            return;
        }

        if (game != settings.game)
            updateTables();

        DataManager.Environment prevEnvironment = environment;
        bool prevExpandAll = expandAll;

        setReloadStyle();

        GUI.enabled = enabled;

        GUILayout.BeginHorizontal();
        {
            environment = (DataManager.Environment)EditorGUILayout.EnumPopup(settings.game + ":", environment);
            if (GUILayout.Button("Reload", reloadStyle) || prevEnvironment != environment)
                updateTables();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("", GUI.skin.horizontalSlider);

        if (error != null && error.Length > 0)
        {
            EditorGUILayout.HelpBox(error, MessageType.Error);
        }
        else if (tables != null)
        {
            expandAll = EditorGUILayout.Toggle("Expand all:", expandAll);

            if (expandAll != prevExpandAll)
            {
                foreach (Table table in tables)
                    table.expanded = expandAll;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            setServiceStyle();
            setLabelStyle();

            foreach (Table table in tables)
            {
                foreach (Field field in table.fields)
                {
                    GUIContent content = new GUIContent("      " + field.name + ": ");
                    Vector2 size = labelStyle.CalcSize(content);

                    if (EditorGUIUtility.labelWidth < size.x)
                        EditorGUIUtility.labelWidth = size.x;
                }
            }

            foreach (Table table in tables)
            {
                GUILayout.BeginHorizontal();
                {
                    table.expanded = EditorGUILayout.Foldout(table.expanded, table.name + " : " + serviceToAccess(table.service));
                    GUILayout.Label(serviceToStr(table.service), serviceStyle);
                }
                GUILayout.EndHorizontal();

                if (table.expanded)
                {
                    foreach(Field field in table.fields)
                    {
                        EditorGUILayout.LabelField("      " + field.name + ":", field.type);
                    }

                    GUILayout.Space(10);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void loadSettings()
    {
        settings = AssetDatabase.LoadAssetAtPath(DataSettings.FILE_DIR, typeof(DataSettings)) as DataSettings;
    }

    void Update()
    {
        if (request != null && request.isDone)
        {
            tables = null;

            if (request.isNetworkError)
            {
                error = request.error;
            }
            else
            {
                try
                {
                    ServerResponse<TableInfo> response = JsonUtility.FromJson<ServerResponse<TableInfo>>(request.downloadHandler.text);

                    if (response.success)
                    {
                        TableInfo tableInfo = response.data;
                        tables = tableInfo.tables;

                        if (tables == null || tables.Length < 1)
                        {
                            error = "No tables found";
                        }
                        else if (expandAll)
                        {
                            foreach (Table table in tables)
                                table.expanded = true;
                        }
                    }
                    else
                        error = response.message;
                }
                catch (Exception e)
                {
                    error = e.Message;
                    Debug.Log(e);
                    Debug.Log(request.downloadHandler.text);
                }
            }

            request = null;
            enabled = true;
        }
    }
    
    private void updateTables()
    {
        if (settings == null || request != null)
            return;

        Dictionary<string, string> body = new Dictionary<string, string>();

        body["game_id"] = settings.gameId.ToString();
        body["environment"] = DataManager.EnvironmentToParam(environment);

        request = UnityWebRequest.Post(DataManager.EnvironmentToApiRootUrl(environment) + "data/v1/get_game_table_desc", DataManager.dictionaryToForm(body));
        request.SetRequestHeader("api-key", DataManager.API_KEY);
        request.Send();

        enabled = false;
        error = null;
        game = settings.game;
    }

    private string serviceToStr(DataManager.Service service)
    {
        switch (service)
        {
            case DataManager.Service.SCI:
                return "SCI";
            case DataManager.Service.RT:
                return "RT";
            case DataManager.Service.ASYNC:
            case DataManager.Service.READ_ONLY:
                return "ASYNC";
            default:
                return "unknown";
        }
    }

    private string serviceToAccess(DataManager.Service service)
    {
        switch (service)
        {
            case DataManager.Service.SCI:
                return "Writable";
            case DataManager.Service.ASYNC:
            case DataManager.Service.RT:
                return "ReadWritable";
            case DataManager.Service.READ_ONLY:
                return "Readable";
            default:
                return "unknown";
        }
    }

    private static void setLabelStyle()
    {
        if (labelStyle == null)
            labelStyle = new GUIStyle(GUI.skin.label);
    }

    private static void setServiceStyle()
    {
        if (serviceStyle != null)
            return;

        serviceStyle = new GUIStyle(GUI.skin.label);
        serviceStyle.alignment = TextAnchor.MiddleRight;
    }

    private static void setReloadStyle()
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
