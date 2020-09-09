using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class ProfileScreenView : MonoBehaviour {
    
    public Text email, fullname, postcode, city, birthdate, gender, employment, displayname, education;
    public Button closebutton;
    public  Button updatebutton;
    public string url;

    Action callback; 

    public void SetUserData ()
    {
        UserProfile profile = UserProfile.instance; 

        email.text = profile.email;
        fullname.text = profile.first_name + " " + profile.last_name;
        postcode.text = profile.postcode;
        city.text = profile.city;
        birthdate.text = profile.birth_year;
        gender.text = profile.gender;
        employment.text = profile.employment;
        displayname.text = profile.user_login;
        education.text = profile.education;
        gender.text = profile.gender;
    }

    public void Awake()
    {
        closebutton.onClick.AddListener(CloseProfile);
        updatebutton.onClick.AddListener(GoToWebsite);
        callback = SetUserData;
        Auth.instance.updateuserprofilewindow = callback;
    }

    private void CloseProfile()
    {
        AuthentificationAnimator.instance.ShowScreen(Auth.AuthStates.Default);
    }

    private void GoToWebsite()
    {
        string target = Auth.instance.Url + url;   
        Application.OpenURL(url);
    }
}
