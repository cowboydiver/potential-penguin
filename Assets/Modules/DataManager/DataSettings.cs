using UnityEngine;

public class DataSettings : ScriptableObject
{
    public const string RESOURCE_DIR = "DataManager/settings";
    public const string FILE_DIR = "Assets/Resources/DataManager/settings.asset";
    public const string FOLDER = "Assets/Resources/DataManager";

    public string game;
    public int gameId;
    public bool offline;
    public DataManager.Environment buildEnvironment = DataManager.Environment.Development;
}
