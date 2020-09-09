using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Facebook.Unity;

[RequireComponent(typeof(Button))]
public class MenuLoginButton : MonoBehaviour {

    public Button loginbutton;
    public Text text;
    Action callback;

    private void Awake()
    {
        callback = updateState;
        loginbutton.onClick.AddListener(OnLoginButton);

    }

  private void Start()
  {
        Auth.instance.updatemainmenu += callback;
        updateState();
  }

  public void updateState()
    {
        if(UserProfile.instance.IsLoggedIn)
        {
            text.text = "Log out"; 
        }
        else
        {
            text.text = "Log in";
        }
    }

    private void OnLoginButton()
    {
        
        if (!UserProfile.instance.IsLoggedIn)
        {
            AuthentificationAnimator.instance.OpenAuth();
        }
        else
        {
            UserProfile.instance.Logout();
            if(FB.IsLoggedIn)
            {
                FB.LogOut();
            }
            updateState();
            GameManager.Inst.UnlockLevels();
        }
    }

}
