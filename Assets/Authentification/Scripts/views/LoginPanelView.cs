using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class LoginPanelView : MonoBehaviour
{

    [SerializeField] private Text statustext;
    [SerializeField] private InputField email;
    [SerializeField] private InputField password;
    //[SerializeField] private InputField username;
    [SerializeField] private Button loginbutton;
    [SerializeField] private Button facebookloginbutton;
    [SerializeField] private Button forgotpasswordbutton;
    [SerializeField] private Button createprofilebutton;
    [SerializeField] private Button cancelbutton;

    public string defaultMessage;
    private Text emailtext;

    public Action<string> serverResponse;

    private EventSystem eventsystem;

    private void Awake()
    {
        loginbutton.onClick.AddListener(OnLogin);

        forgotpasswordbutton.onClick.AddListener(OnForgotPassword);
        createprofilebutton.onClick.AddListener(OnCreateNewAccount);
        cancelbutton.onClick.AddListener(OnCancel);

        eventsystem = EventSystem.current;

        serverResponse = serverErrorResponse;

        emailtext = email.transform.Find("Text").GetComponent<Text>();

        statustext.color = Color.black;
        statustext.text = defaultMessage;

        #if UNITY_STANDALONE || UNITY_STANDALONE_OSX
            facebookloginbutton.gameObject.SetActive(false);
        #else
            //facebookloginbutton.onClick.AddListener(OnFacebookLogin);
        #endif
    }

    private void OnLogin ()
    {
        statustext.text = "";
        emailtext.color = Color.black;

        if (ValidateFields())
        {
            WWWForm userdata = new WWWForm();
            userdata.AddField("username", email.text);
            userdata.AddField("password", password.text);

            print(("Login: email = " + email.text));
            Auth.instance.CallServer(Auth.AuthStates.Login, userdata, serverResponse);
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = eventsystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if(next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(eventsystem));  //if it's an input field, also set the text caret

                    eventsystem.SetSelectedGameObject(next.gameObject, new BaseEventData(eventsystem));
            }
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (ValidateFields())
            {
                OnLogin();
            }
        }
    }



    //called when forgot password button is pressed. Opens reset screen
    private void OnForgotPassword ()
    {
        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Forgot);
    }

    private void OnCreateNewAccount()
    {
        email.text = "";
        emailtext.color = Color.black;
        statustext.color = Color.black;
        password.text = "";
        statustext.text = defaultMessage; 

        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Signup);
    }

    //callback from auth
    public void serverErrorResponse (string msg = "")
    {
        statustext.color = Color.red;
        statustext.text = msg;
    }

    private void OnCancel ()
    {
        email.text = "";
        emailtext.color = Color.black;
        password.text = "";
        statustext.text = defaultMessage;
        statustext.color = Color.black;

        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
    }

    private bool ValidateFields()
    {
        statustext.color = Color.red;
        statustext.text = "";
        if (!Auth.instance.IsValidEmail(email.text))
        {
            if(emailtext.text.Length == 0)
            {
                statustext.text = "Aww come on! You forgot the email";
                statustext.color = Color.black;
            }
            else
            {
                statustext.text = "Invalid email.";
            }

            emailtext.color = Color.red;
            return false;
        }
        else if (password.text.Length == 0)
        {
            statustext.text = "Enter a password";
            return false;
        }
        return true;
    }
}
