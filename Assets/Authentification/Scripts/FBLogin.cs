using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;

public class FBLogin : MonoBehaviour {
    
    List<string> perms = new List<string>() { "public_profile", "email", "user_birthday" }; // "user_birthday", "user_games_activity", "user_education_history", "user_friends", "user_location"

    private void Awake()
    {

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init();
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void Login()
    {
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    void Logout()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        //print("login callback for Facebook");
        if(FB.IsLoggedIn && result.Error == null)
        {
            print("User is logged in");
            UserProfile.instance.Login();
            AccessToken aToken = result.AccessToken;

            WWWForm fbdata = new WWWForm();
            fbdata.AddField("fb_id", aToken.UserId.ToString());
            fbdata.AddField("access_token", aToken.ToString());
            fbdata.AddField("expiration_date", aToken.ExpirationTime.ToString());

            Auth.instance.CallServer(Auth.AuthStates.Facebook, fbdata);
        }
    }
}
