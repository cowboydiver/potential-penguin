using UnityEditor;

[CustomEditor(typeof(DataSettings))]
public class DataSettingsEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DataSettings settings = target as DataSettings;
        EditorGUILayout.LabelField("Game:", settings.game);
        EditorGUILayout.LabelField("Game Id:", settings.gameId.ToString());
        EditorGUILayout.LabelField("Build Mode:", settings.buildEnvironment.ToString());
        EditorGUILayout.LabelField("Offline:", settings.offline.ToString());
    }
}
