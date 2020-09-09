using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class WebProxy : MonoBehaviour
{
    private static WebProxy _instance;

    public static WebProxy instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(WebProxy)) as WebProxy;
                if (_instance == null)
                {
                    GameObject go = new GameObject("Webproxy");
                    _instance = go.AddComponent<WebProxy>();
                }
            }
            return _instance;
        }
    }

    [DllImport("__Internal")]
    private static extern void _post(string url, string str, string key, int id = -1);

    private class WebCallback
    {
        private static List<WebCallback> list = new List<WebCallback>();
        private static int nextId = 0;

        private int id;
        private DataPromise<string> promise;

        public WebCallback(DataPromise<string> promise)
        {
            this.promise = promise;
            id = nextId++;
            list.Add(this);
        }

        public int GetId()
        {
            return id;
        }

        public void Fulfill(string response)
        {
            removeSelf();
            promise.Fulfill(response);
        }

        public void Reject(Exception e)
        {
            removeSelf();
            promise.Reject(e);
        }

        private void removeSelf()
        {
            list.Remove(this);
        }

        public static WebCallback Get(int id)
        {
            foreach (WebCallback entry in list)
                if (entry.GetId() == id)
                    return entry;
            return null;
        }
    }

    [Serializable]
    public class WebResponse
    {
        public int id;
        public string message;
        public string response;
        public bool success;
    }

    public void OnCallback(string response)
    {
        WebResponse webResponse = JsonUtility.FromJson<WebResponse>(response);
        WebCallback callback = WebCallback.Get(webResponse.id);

        if (callback == null)
            return;
        if (webResponse.success)
            callback.Fulfill(webResponse.response);
        else
            callback.Reject(new Exception(webResponse.message));
    }

    public static void Post(string url, Dictionary<string, string> dictionary)
    {
        _post(url, formatParams(dictionary), DataManager.API_KEY);
    }

    public static void Get(string url, Dictionary<string, string> dictionary, DataPromise<string> promise)
    {
        if (instance != null)
            _post(url, formatParams(dictionary), DataManager.API_KEY,(new WebCallback(promise)).GetId());
        else
            promise.Reject(new Exception("Failed to create GameObject for WebProxy. Will be unable to receive server response."));
    }

    public static void Get(string url, DataPromise<string> promise)
    {
        Get(url, null, promise);
    }

    private static string formatParams(Dictionary<string, string> dictionary)
    {
        if (dictionary == null)
            return "";

        string[] arr = new string[dictionary.Count];
        int i = 0;

        foreach (KeyValuePair<string, string> entry in dictionary)
            arr[i++] = WWW.EscapeURL(entry.Key) + "=" + WWW.EscapeURL(entry.Value);

        return string.Join("&", arr);
    }
}