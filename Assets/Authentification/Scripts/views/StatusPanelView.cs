using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System;

public class StatusPanelView : MonoBehaviour {

    public Text messagetext;
    public Button closebutton;
    public Action<string> updatemessage;

  private void Awake()
  {
        closebutton.onClick.AddListener(OnCloseClicked);
        updatemessage = UpdateMessagefield;
        Auth.instance.updatemessageaction = updatemessage;
  }

    private void OnCloseClicked()
    {
        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
    }

    private void UpdateMessagefield(string msg)
    {
        print("Update message field");
        messagetext.text = msg; 
        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Message);
    }
}
