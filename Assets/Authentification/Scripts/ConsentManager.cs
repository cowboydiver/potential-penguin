using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using SimpleJSON;
using System.Text.RegularExpressions; 

public class ConsentManager : MonoBehaviour {

    [SerializeField] private string GetConsentDataEndPoint;
    [SerializeField] private string HasConsentedEndPoint;
    [SerializeField] private string ConsentEndPoint;

    public Action<string, string> updateconsentform;

    [SerializeField]
    public enum ConsentStates
    {
        GetConsentForm,
        Update,
        HasConsented
    }

    public ConsentStates currentstate; 

    #region singleton stuff
    private static ConsentManager _instance;
    public static ConsentManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ConsentManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }

    }
    #endregion

    public void CallServer(WWWForm form, ConsentStates state, Action<string,string> action = null)
    {
        currentstate = state;
        //updateconsentform = action;
        StartCoroutine(RequestServer(form));
    }

    private string MakeUrl()
    {
        string target = "http://timo.sahdev.org/scienceathome-v6/wp-json/auth/v1/"; //Auth.instance.Url;

        switch(currentstate)
        {
            case ConsentStates.GetConsentForm :
                target += GetConsentDataEndPoint;
                break;
            case ConsentStates.HasConsented :
                target += HasConsentedEndPoint;
                break;
            case ConsentStates.Update :
                target += ConsentEndPoint;
                break;
        }

        return target;
    }

    IEnumerator RequestServer(WWWForm form)
    {
        UnityWebRequest r = UnityWebRequest.Post(MakeUrl(), form);
        r.SetRequestHeader("api-key", Auth.instance.Apikey);

        AuthentificationAnimator.instance.SetLoader(true);

        yield return r.Send();

        if (r.isNetworkError)
        {
            print("error: " + r.error.ToString());
            //something fucked up
        }
        else
        {
            print("Server has responded");
            AuthentificationAnimator.instance.SetLoader(false);
            switch (currentstate)
            {
                case ConsentStates.GetConsentForm :
                    ConsentFormData(r);
                    break;
                case ConsentStates.HasConsented :
                    OnHasConsented(r);
                    break;
                case ConsentStates.Update :
                    OnUpdateConsent(r);
                    break;
            }
        }
    }

    void ConsentFormData (UnityWebRequest request)
    {
        print("get consent form data:");
        print(request.downloadHandler.text); 
        JSONNode res = JSON.Parse(request.downloadHandler.text);
        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        if (success)
        {
            string content = res["data"]["description"];
            string title = res["data"]["title"];
            updateconsentform(title, content);
            AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Consent);
        }
        else
        {
            
        }

    }

    void OnHasConsented(UnityWebRequest request)
    {
        
        JSONNode res = JSON.Parse(request.downloadHandler.text);
        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        if(success)
        {
            bool consented = res["data"]["consented"].AsBool;
            if(consented)
            {
                AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
            }
            else
            {
                WWWForm www = new WWWForm();
                www.AddField("game_id", Auth.instance.Gameid);
                CallServer(www,ConsentManager.ConsentStates.GetConsentForm,updateconsentform);
                AuthentificationAnimator.instance.SetLoader(true);
                //get consentform
            }
        }
        {
            
        }
       
    }

    void OnUpdateConsent(UnityWebRequest request)
    {
        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
    }

}
