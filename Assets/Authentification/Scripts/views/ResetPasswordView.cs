using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPasswordView : MonoBehaviour {

    public string defaultMessage; 
    public Text statustext;
    private Text emailtext; 
    public InputField email;
    public Button submitbutton;
    public Button cancelbutton; 

    private void Awake()
    {
        submitbutton.onClick.AddListener(OnSubmit);
        cancelbutton.onClick.AddListener(OnCancel);

        emailtext = email.transform.Find("Text").GetComponent<Text>();
        statustext.color = Color.black;
        statustext.text = defaultMessage;
    }

    private void OnSubmit()
    {
        if (Auth.instance.IsValidEmail(email.text))
        { 
            WWWForm mf = new WWWForm();
            mf.AddField("email", email.text);
            Auth.instance.CallServer(Auth.AuthStates.Forgot, mf);
        }
        else
        {
            statustext.text = "The email is not valid.";
            emailtext.color = Color.red;
            statustext.color = Color.red;
        }
    }

    private void OnCancel()
    {
        email.text = "";
        emailtext.color = Color.black;
        statustext.color = Color.black;
        statustext.text = defaultMessage;

        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
    }
}
