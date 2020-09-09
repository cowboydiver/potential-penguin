using UnityEngine;
using System;
using SimpleJSON;

public class UserProfile : MonoBehaviour {
    public int ID;
    public string first_name;
    public string last_name;
    public string nickname;
    public string user_nicename;
    public string email;
    public string postcode;
    public string city;
    public string country;
    public string birth_day;
    public string birth_month;
    public string birth_year;
    public string gender;
    public string education;
    public string employment;
    public string profile_pic_url;
    public string auth_mode;
    public string parse_user_ids;
    public string user_login;
    public string parse_email;
    public string fb_user_id;
    public string fb_location;
    public string fb_email;
    public int profile_picture_id;
    public int employee_id;
    public string roles;
    public bool skip_sync;
    public int user_registered_u;
    public string user_registered;
    private bool isLoggedIn;

    #region singleton stuff

    private static UserProfile _instance;
    public static UserProfile instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UserProfile>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

  public bool IsLoggedIn
  {
    get
    {
            int val = PlayerPrefs.GetInt("loggedin");
             if(val == 0)
             {
                isLoggedIn = false;
             }
              else
             {
                isLoggedIn = true;
             }
            return isLoggedIn;
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
            
            if(PlayerPrefs.HasKey("loggedin"))
            PlayerPrefs.SetInt("loggedin", 0);
        }
    }
    #endregion

    public void SaveToDisc()
    {
        PlayerPrefs.SetInt("ID", ID);
        PlayerPrefs.SetString("first_name", first_name);
        //PlayerPrefs.SetString();
    }

    public void Login()
    {
        PlayerPrefs.SetInt("loggedin", 1);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("ID");
        PlayerPrefs.DeleteKey("email");

        PlayerPrefs.SetInt("loggedin",0);
    }


}


