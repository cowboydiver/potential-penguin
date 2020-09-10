using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public interface Writable { }

public interface Readable { }

public interface ReadWritable : Writable, Readable { }

[Serializable]
public class DataManager : Writable
{
    public const string API_ROOT_URL_DEV = "http://daniel.sahdev.org/scienceathome-v6/wp-json/";
    public const string API_ROOT_URL_STAGING = "http://daniel.sahdev.org/scienceathome-v6/wp-json/";
    public const string API_ROOT_URL_PRODUCTION = "http://daniel.sahdev.org/scienceathome-v6/wp-json/";
    public const string API_KEY = "FunkyScienceAPPles";
    public const string MODULE_VERSION = "0.0.1";
    private const string INSTALL_ID_KEY = "DataManager_installId";

    public enum Service
    {
        SCI,
        ASYNC,
        RT,
        READ_ONLY
    }

    public enum Environment
    {
        Production,
        Staging,
        Development,
    }

    public class DataManagerController : MonoBehaviour
    {
        private DataSettings settings;
        private string apiRootUrl;
        private string environmentParam;

        void Awake()
        {
            settings = Resources.Load<DataSettings>(DataSettings.RESOURCE_DIR);
            Environment environment = GetEnvironment();

            apiRootUrl = EnvironmentToApiRootUrl(environment);
            environmentParam = EnvironmentToParam(environment);
        }

        public IEnumerator ServerRequest(string url, WWWForm form, DataPromise<string> promise)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("api-key", API_KEY);
            yield return request.Send();

            if (request.isNetworkError)
                promise.Reject(new Exception(request.error));
            else
                promise.Fulfill(request.downloadHandler.text);
        }

        public bool IsOffline()
        {
            return settings.offline;
        }

        public string GetSaveEndpoint()
        {
            return apiRootUrl + "data/v1/save_data";
        }

        public int GetGameId()
        {
            return settings.gameId;
        }

        public string GetEnvironmentParam()
        {
            return environmentParam;
        }

        public Environment GetEnvironment()
        {
#if UNITY_EDITOR
            return settings.buildEnvironment == Environment.Production ? Environment.Staging : settings.buildEnvironment;
#else
            return settings.buildEnvironment;
#endif
        }
    }

    [Serializable]
    private class PendingData
    {
        public string table_name;
        public string data;
        public long client_time;

        public PendingData(Writable obj)
        {
            table_name = obj.GetType().ToString();
            data = JsonUtility.ToJson(obj);
            client_time = getUnixTimestampMillis();
        }

        public PendingData(string label, string value)
        {
            table_name = "string";
            data = JsonUtility.ToJson(new LabelledEvent(label, value));
            client_time = getUnixTimestampMillis();
        }
    }

    [Serializable]
    private class SessionData
    {
        public string session_token;
        public string install_id;

        public SessionData(string session_token, string install_id)
        {
            this.session_token = session_token;
            this.install_id = install_id;
        }
    }

    [Serializable]
    private class LabelledEvent
    {
        public string label;
        public string value;

        public LabelledEvent(string label, string value)
        {
            this.label = label;
            this.value = value != null ? value : "";
        }
    }

    private static string sessionToken;
    private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static DataManagerController _instance;
    public static DataManagerController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindObjectOfType(typeof(DataManagerController)) as DataManagerController;
                if (_instance == null)
                {
                    GameObject go = new GameObject("DataManager");
                    _instance = go.AddComponent<DataManagerController>();
                }
            }
            return _instance;
        }
    }

    private static string _installId;
    public static string installId
    {
        get
        {
            if (_installId == null)
                _installId = PlayerPrefs.GetString(INSTALL_ID_KEY, "");
            return _installId;
        }
    }

    [SerializeField]
    private List<PendingData> list = new List<PendingData>();
    [SerializeField]
    private int game_id;
    [SerializeField]
    private string environment;

    public static DataPromise<ServerResponse> Save(Writable obj)
    {
        DataPromise<ServerResponse> promise = new DataPromise<ServerResponse>();

        sendRequest(instance.GetSaveEndpoint(), formatParams(obj))
        .Then(text => onSaveReturned(text, promise))
        .Catch(err => promise.Reject(err));

        return promise;
    }

    public static DataPromise<ServerResponse> Save<T>(List<T> objects) where T : Writable
    {
        return (new DataManager()).Append(objects).Flush();
    }

    public static DataPromise<ServerResponse> Save<T>(T[] objects) where T : Writable
    {
        return Save(new List<T>(objects));
    }

    public static DataPromise<ServerResponse> Save<T>(string label, T value)
    {
        return Save(label, value.ToString());
    }

    public static DataPromise<ServerResponse> Save(string label, string value = null)
    {
    DataPromise<ServerResponse> promise = new DataPromise<ServerResponse>();

        sendRequest(instance.GetSaveEndpoint(), formatParams(label, value))
        .Then(text => onSaveReturned(text, promise))
        .Catch(err => promise.Reject(err));

        return promise;
    }

    public DataManager Append(Writable obj)
    {
        list.Add(new PendingData(obj));
        return this;
    }

    public DataManager Append<T>(T[] objects) where T : Writable
    {
        return Append(new List<T>(objects));
    }

    public DataManager Append<T>(List<T> objects) where T: Writable
    {
        foreach (Writable obj in objects)
            Append(obj);
        return this;
    }

    public DataManager Append<T>(string label, T value)
    {
        return Append(label, value.ToString());
    }

    public DataManager Append(string label, string value = null)
    {
        list.Add(new PendingData(label, value));
        return this;
    }

    public DataPromise<ServerResponse> Flush()
    {
        DataPromise<ServerResponse> promise = new DataPromise<ServerResponse>();

        sendRequest(instance.GetSaveEndpoint(), formatParams(this))
        .Then(text => onSaveReturned(text, promise))
        .Catch(err => promise.Reject(err));

        list = new List<PendingData>();

        return promise;
    }

    public static DataPromise<T> Get<T>() where T : class, Readable
    {
        DataPromise<T> promise = new DataPromise<T>();
        return promise;
    }

    public static DataPromise<T> Find<T>(DataQuery query) where T : class, Readable
    {
        DataPromise<T> promise = new DataPromise<T>();
        return promise;
    }

    public static DataPromise<T> First<T>(DataQuery query) where T : class, Readable
    {
        DataPromise<T> promise = new DataPromise<T>();
        return promise;
    }

    public static DataPromise<int> Count<T>(DataQuery query) where T : class, Readable
    {
        DataPromise<int> promise = new DataPromise<int>();
        return promise;
    }

    private static Dictionary<string, string> formatParams(Writable obj)
    {
        Dictionary<string, string> dictionary = formatParams();

        dictionary["table_name"] = obj.GetType().ToString();
        dictionary["data"] = JsonUtility.ToJson(obj);

        return dictionary;
    }

    private static Dictionary<string, string> formatParams(string label, string value)
    {
        Dictionary<string, string> dictionary = formatParams();

        dictionary["table_name"] = "string";
        dictionary["data"] = JsonUtility.ToJson(new LabelledEvent(label, value));

        return dictionary;
    }

    private static Dictionary<string, string> formatParams()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        dictionary["client_time"] = getUnixTimestampMillis().ToString();
        dictionary["game_id"] = instance.GetGameId().ToString();
        dictionary["environment"] = instance.GetEnvironmentParam();
        dictionary["install_id"] = installId;
        dictionary["auth_token"] = "";
        dictionary["session_token"] = sessionToken != null ? sessionToken : "";

        return dictionary;
    }

    private static long getUnixTimestampMillis()
    {
        return (long)(DateTime.UtcNow - unixEpoch).TotalMilliseconds;
    }

    public static WWWForm dictionaryToForm(Dictionary<string, string> dictionary)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> entry in dictionary)
            form.AddField(entry.Key, entry.Value);
        return form;
    }

    private static DataPromise<string> sendRequest(string url, Dictionary<string, string> dictionary)
    {
        DataPromise<string> promise = new DataPromise<string>();

        if (instance.IsOffline())
        {
            string[] arr = new string[dictionary.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> field in dictionary)
                arr[i++] = field.Key + ": " + field.Value;
            Debug.Log("Saved: {" + string.Join(", ", arr) + "}");
        }
        else
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebProxy.Get(url, dictionary, promise);
#else
            instance.StartCoroutine(instance.ServerRequest(url, dictionaryToForm(dictionary), promise));
#endif
        }

        return promise;
    }

    private static void onSaveReturned(string text, DataPromise<ServerResponse> promise)
    {
        ServerResponse<SessionData> response;

        try
        {
            response = JsonUtility.FromJson<ServerResponse<SessionData>>(text);
        }
        catch
        {
            Debug.Log(text);
            promise.Reject(new Exception("[Server error] Unexpected error. Inform a backend developer."));
            return;
        }

        try
        {

            if (!response.success)
                promise.Reject(new Exception("[Server error] " + response.message));
            else {

                if (installId.Length == 0 && response.data.install_id != null && response.data.install_id != installId)
                {
                    PlayerPrefs.SetString(INSTALL_ID_KEY, response.data.install_id);
                    _installId = null;
                }

                if (sessionToken == null && sessionToken != response.data.session_token && response.data.session_token.Length > 0)
                    sessionToken = response.data.session_token;

                promise.Fulfill(response);
            }
        }
        catch (Exception e)
        {
            promise.Reject(e);
        }
    }

    public static string EnvironmentToParam(Environment environment)
    {
        switch (environment)
        {
            case Environment.Production:
                return "Live";
            case Environment.Development:
                return "Dev";
            case Environment.Staging:
                return "Staging";
        }
        return null;
    }

    public static string EnvironmentToApiRootUrl(Environment environment)
    {
        switch (environment)
        {
            case Environment.Production:
                return API_ROOT_URL_PRODUCTION;
            case Environment.Development:
                return API_ROOT_URL_STAGING;
            case Environment.Staging:
                return API_ROOT_URL_DEV;
        }
        return null;
    }

    public static string FormatBody(Dictionary<string, string> body)
    {
        string[] arr = new string[body.Count];
        int i = 0;

        foreach (KeyValuePair<string, string> field in body)
            arr[i++] = field.Key + "=" + field.Value;

        return string.Join("&", arr);
    }
}
