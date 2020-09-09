using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using SimpleJSON;
using System.Text.RegularExpressions; 

public class Auth : MonoBehaviour {
    
    #region singleton stuff
    private static Auth _instance;
    public static Auth instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Auth>();
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
        InitUrl();
    }
    #endregion

    [SerializeField] private string developerApiKey;
    [SerializeField] private string developerApiUrl;

    [SerializeField] private string stagingApiKey;
    [SerializeField] private string stagingApiUrl;

    [SerializeField] private string liveApiKey;
    [SerializeField] private string liveApiUrl;
    [SerializeField] private bool _IsConsentActive;
    [SerializeField] private int gameid;


    private string apikey;

  public string Apikey
  {
    get
    {
      return apikey;
    }
  }

  private string url;

  public string Url
  {
    get
    {
      return url;
    }
  }

  public int Gameid
  {
    get
    {
      return gameid;
    }
  }

    public bool IsConsentActive
    {
        get
        {
            return _IsConsentActive;
        }
    }

    public bool IsLoggedIn
    {
        get
        {
            return isLoggedIn;
        }
    }

    public Action<string> callbackaction;
    public Action<string> updatemessageaction;
    public Action<string, string> updateconsentform;
    public Action updateuserprofilewindow;
    public Action updatemainmenu;

    public enum LiveState
    {
        DEVELOPER,
        STAGING,
        LIVE
    }

    public enum AuthStates
    {
        Login,
        Forgot,
        Signup,
        ResetPassword,
        Consent,
        Facebook,
        Default,
        Message,
        GetConsentData,
        UserProfile,
        HasConsented
    }

    public AuthStates currentstate;
    [SerializeField]private LiveState livestate; 

    [SerializeField] private string LoginEndPoint;
    [SerializeField] private string ResetLoginEndPoint;
    [SerializeField] private string SignupEndPoint;
    [SerializeField] private string FacebookEndPoint;
    [SerializeField] private string UserProfileEndPoint;

    public bool isLoggedIn;

    #region url handling
    private void InitUrl()
    {
        switch (livestate)
        {
            case LiveState.DEVELOPER:
                url = developerApiUrl;
                apikey = developerApiKey;
                break;
            case LiveState.STAGING:
                url = stagingApiUrl;
                apikey = stagingApiKey;
                break;
            case LiveState.LIVE:
                url = liveApiUrl;
                apikey = liveApiKey;
                break;
        }
    }

    public bool IsValidEmail(string strIn)
    {
        // Return true if strIn is in valid e-mail format.
        return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

    private string GetTargetURL (AuthStates state)
    {
        string target = url;

        switch (state)
        {
            case AuthStates.Login :
                target += LoginEndPoint;
                break;
            case AuthStates.Forgot :
                target += ResetLoginEndPoint;
                break;
            case AuthStates.Signup :
                target += SignupEndPoint;
                break;
            case AuthStates.Facebook :
                target += FacebookEndPoint;
                break;
            case AuthStates.UserProfile :
                target += UserProfileEndPoint;
                break;
        }
        return target;
    }
    #endregion

    public void OpenUserProfile()
    {
        WWWForm v = new WWWForm(); 
        v.AddField("wp_user_id", UserProfile.instance.ID);
    }
     
    public void CallServer(AuthStates state, WWWForm form, Action<string> action = null)
    {
        print("Auth.CallServer: " + state);
        currentstate = state;
        callbackaction = action;
        StartCoroutine(RequestServer(form));
    }

    IEnumerator RequestServer(WWWForm form)
    {
        print("Server requested: " + GetTargetURL(currentstate)); 
        UnityWebRequest r = UnityWebRequest.Post(GetTargetURL(currentstate), form);
        r.SetRequestHeader("api-key", apikey);
        //r.SetRequestHeader("Origin", "https://scienceathome.org");
        //print("Api-key:" + )
        AuthentificationAnimator.instance.SetLoader(true);

        yield return r.Send();

        if (r.isError)
        {
            print("error: " + r.error.ToString());
            //something fucked up
        }
        else
        {
            print("Server has responded");
            AuthentificationAnimator.instance.SetLoader(false);
            switch(currentstate)
            {
                case AuthStates.Login :
                    OnLoginResponse(r);
                break;
                case AuthStates.Signup:
                    OnSignupResponse(r);
                    break;
                case AuthStates.Forgot :
                    OnForgotMailResponse(r);
                    break;
                case AuthStates.Facebook :
                    OnFaceBookLogin(r);
                    break;
  
                case AuthStates.UserProfile :
                    OnGetUserDataResponse(r);
                    break;
            }
        }
    }

    private void OnFaceBookLogin(UnityWebRequest request)
    {
        print("response");
        print(request.downloadHandler.text);

        JSONNode res = JSON.Parse(request.downloadHandler.text);

        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        if(success)
        {
            //

        }

        AuthentificationAnimator.instance.CloseAuth();
    }

    private void OnGetUserDataResponse(UnityWebRequest request)
    {
        Debug.Log("userdata response: " + request.downloadHandler.text);
        JSONNode res = JSON.Parse(request.downloadHandler.text);
        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        UserProfile.instance.ID = res["data"]["ID"].AsInt;
        UserProfile.instance.email = res["data"]["email"];
        UserProfile.instance.nickname = res["data"]["nickname"];
        UserProfile.instance.first_name = res["data"]["first_name"];
        UserProfile.instance.last_name = res["data"]["last_name"];
        UserProfile.instance.user_nicename = res["data"]["user_nicename"];
        UserProfile.instance.postcode = res["data"]["postcode"];
        UserProfile.instance.city = res["data"]["city"];
        UserProfile.instance.country = res["data"]["country"];
        UserProfile.instance.profile_pic_url = res["data"]["profile_pic_url"];
        UserProfile.instance.education = res["data"]["education"];
        UserProfile.instance.birth_day = res["data"]["birth_day"];
        UserProfile.instance.birth_month = res["data"]["birth_month"];
        UserProfile.instance.birth_year = res["data"]["birth_year"];
        UserProfile.instance.employment = res["data"]["employment"];
        UserProfile.instance.user_login = res["data"]["user_login"];
        UserProfile.instance.gender = res["data"]["gender"];

        updateuserprofilewindow();

        AuthentificationAnimator.instance.ShowScreen(AuthStates.UserProfile);
    }

    private void OnLoginResponse(UnityWebRequest request)
    {
        print("response");
        print(request.downloadHandler.text);

        JSONNode res = JSON.Parse(request.downloadHandler.text);

        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        if(success)
        {
            if(msg == "Login successful...") //login success please get this right
            {
                //shut down AuthCanvas
                callbackaction = null;

                UserProfile.instance.ID = res["data"]["ID"].AsInt;
                UserProfile.instance.email = res["data"]["user_email"];
                UserProfile.instance.nickname = res["data"]["user_login"];
                UserProfile.instance.Login();

                if(IsConsentActive)
                {
                    //check for consent
                    WWWForm c = new WWWForm(); 
                    c.AddField("game_id", gameid);
                    //c.AddField("wp_user_id", UserProfile.instance.ID);
                    ConsentManager.instance.CallServer(c, ConsentManager.ConsentStates.GetConsentForm);
                    //ConsentManager.instance.CallServer(c, ConsentManager.ConsentStates.HasConsented);
                    updatemainmenu();
                }
                else
                {
                    AuthentificationAnimator.instance.CloseAuth();
                    updatemainmenu();
                }
            }
        }
        else
        {
            if (code == "unknown_error") //wrong email
            {
                callbackaction("Wrong email. Try again.");
            }
            if(code == "invalid_login")
            {
                callbackaction("Wrong email or password. Try again");
            }
        }
    }

    private void OnSignupResponse (UnityWebRequest request)
    {
        Debug.Log("signup response: " + request.downloadHandler.text);
        JSONNode res = JSON.Parse(request.downloadHandler.text);
        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        if(success)
        {
            if(msg == "Signup successful") //signupsuccess go-to 
            {
                //callbackaction = null;
                string messagestring = "Signup successful. We have sent you a confirmation email.\nPlease click the confirmation link to " +
                    "complete your registration.";
                //callbackaction(msg);
                updatemessageaction(messagestring);
                AuthentificationAnimator.instance.ShowScreen(AuthStates.Message);

                //show message popup with message sucessful
            }
            if(code == "Missing parameter(s): password") // error. send back message to SignupScreen
            {
                callbackaction(msg);
                callbackaction = null;
            }
        }
        else
        {
            if(msg == "Account already exists for this email.")
            {
                callbackaction(msg);
            }
        }
    }

    private void OnForgotMailResponse(UnityWebRequest request)
    {
        Debug.Log("forgot mail response: " + request.downloadHandler.text);
        //print("OnForgtMeailResponse called");
        JSONNode res = JSON.Parse(request.downloadHandler.text);
        string code = res["code"];
        bool success = res["success"].AsBool;
        string msg = res["message"];

        if(success)
        {
            print("Reset successful");
            updatemessageaction(msg);
        }
        else
        {
            if(code == "invalid_login")
            {
                print("Invalid login");
                updatemessageaction("No user found for this email.");
            }
        }
    }

    public void OpenMessage(string val)
    {
        print("Auth.instance.SendMessage('" + val + "');"); 
        updatemessageaction(val);
    }

}
