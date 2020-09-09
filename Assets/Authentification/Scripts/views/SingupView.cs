using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SingupView : MonoBehaviour {

    public string defaultmessage; 
    public Text statustext;
    public InputField email;
    public InputField password;
    public InputField displayname;
    public Dropdown gender;
    public Dropdown birthyear;

    public Button submitbutton;
    public Button cancelbutton;
    public Action<string> serverresponse;

    public Toggle newsletter;

    private List<string> years = new List<string>(); 

    private Text emailtext;
    private EventSystem eventsystem;

    public Action<string> serverResponse; 

    private void Awake()
    {
        submitbutton.onClick.AddListener(OnSubmit);
        cancelbutton.onClick.AddListener(OnCancel);
        serverresponse = serverResponseCallback;
        statustext.color = Color.black;
        statustext.text = defaultmessage; 

        emailtext = email.transform.Find("Text").GetComponent<Text>();
        serverResponse += serverErrorResponse;

        MakeYears();
    }

    void MakeYears ()
    {
        int current = 2012; 

        while (current > 1914)
        {
            birthyear.options.Add(new Dropdown.OptionData(current.ToString()));
            current--;
        }
    }

    //callback from auth
    public void serverErrorResponse(string msg = "")
    {
        statustext.color = Color.red;
        statustext.text = msg;
    }

    void OnSubmit()
    {
        if(ValidateFields())
        {
            WWWForm sd = new WWWForm(); 
            sd.AddField("email", email.text);
            sd.AddField("password", password.text);
            sd.AddField("displayname", displayname.text);
            sd.AddField("gender", gender.options[gender.value].text.ToLower());

            sd.AddField("birth_year", birthyear.options[birthyear.value].ToString().ToLower());

            sd.AddField("newsletter_opt_in", newsletter.isOn.ToString());

            Auth.instance.CallServer(Auth.AuthStates.Signup, sd, serverresponse);
        }
    }



    void OnCancel()
    {
        emailtext.color = Color.black;
        email.text = "";
        password.text = "";
        displayname.text = "";
        gender.value = 0;
        birthyear.value = 0;
        newsletter.isOn = false;
        statustext.text = defaultmessage;
        statustext.color = Color.black;

        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = eventsystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(eventsystem));  //if it's an input field, also set the text caret

                    eventsystem.SetSelectedGameObject(next.gameObject, new BaseEventData(eventsystem));
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ValidateFields())
            {
                OnSubmit();
            }
        }
    }

    private bool ValidateFields()
    {
        statustext.text = "";
        if (!Auth.instance.IsValidEmail(email.text))
        {
            statustext.text = "Invalid email.";
            statustext.color = Color.red;
            emailtext.color = Color.red;
            return false;
        }
        else if (password.text.Length == 0)
        {
            statustext.text = "Enter a password";
            statustext.color = Color.red;
            return false;
        }
        else if (displayname.text.Length == 0)
        {
            statustext.text = "Enter a username";
            statustext.color = Color.red;
            return false;
        }

        return true;
    }

    public void serverResponseCallback (string msg)
    {
        statustext.text = msg;
    }
}
