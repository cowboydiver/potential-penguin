using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System;

public class OpenProfileButton : MonoBehaviour {

    public Button button;

  private void Awake()
  {
        button.onClick.AddListener(OpenProfile);
  }

    void OpenProfile ()
    { 
        if(UserProfile.instance.IsLoggedIn)
        {
            WWWForm form = new WWWForm();
            form.AddField("email", UserProfile.instance.email);
            Auth.instance.CallServer(Auth.AuthStates.UserProfile, form);
        }
        else
        {
            Auth.instance.OpenMessage("You have to be logged in to view your profile");
        }
    }
}
