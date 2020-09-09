using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConsentForm : MonoBehaviour {

    public Text header;
    public Text content;
    public Button acceptbutton;
    public Button denybutton;
    public Scrollbar scroll;

    public Action<string,string> updatemessage;

    void Awake()
    {
        //scroll.onValueChanged.AddListener(OnScrolled);
        acceptbutton.onClick.AddListener(OnAccept);
        denybutton.onClick.AddListener(OnDeny);
        acceptbutton.interactable = false;

        updatemessage = SetContent;
        //Auth.instance.updateconsentform = updatemessage;
        ConsentManager.instance.updateconsentform = updatemessage;
        print("Consentmanager: consentmanager awake");
    }

    public void SetContent(string headertext, string contenttext)
    {
        string description = contenttext.Replace("\n\n", "\n");
        header.text = headertext;
        content.text = contenttext;
        acceptbutton.interactable = false;
    }

    public void OnScrolled ()
    {
        //print("On scroll: " + scroll.value);

        if (scroll.value < 0.2f)
        {
            acceptbutton.interactable = true; 
        }
        else
        {
            //acceptbutton.interactable = false; 
        }
    }

    void OnAccept ()
    {
        SendConsent(true);
    }

    void OnDeny()
    {
        SendConsent(false);
    }

    /*
     * not to be implemented
     */
    void SendConsent(bool consent)
    {
        //TODO
        WWWForm cf = new WWWForm();
        cf.AddField("game_id", "insert game _id");
        cf.AddField("wp_user_id", "insert_user_id");
        cf.AddField("consented", consent.ToString());

        ConsentManager.instance.CallServer(cf, ConsentManager.ConsentStates.Update);
        //Auth.instance.CallServer(Auth.AuthStates.Consent);

        //AuthentificationAnimator.instance.CloseAuth();
    }
}
