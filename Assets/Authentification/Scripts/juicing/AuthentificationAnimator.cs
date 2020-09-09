using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class AuthentificationAnimator : MonoBehaviour {

    public CanvasGroup loader;
    public CanvasGroup LoginPanel;
    public CanvasGroup StatusPanel;
    public CanvasGroup Signup;
    public CanvasGroup ForgotPassword;
    //public CanvasGroup ConsentForm;
    public CanvasGroup MainCanvas;
    public CanvasGroup UserProfile;

    private Auth.AuthStates currentstate;
    private Auth.AuthStates nextstate;

    private CanvasGroup fadein;
    private CanvasGroup fadeout;

    private float fadespeed = 0.4f; 

    #region singleton stuff
    private static AuthentificationAnimator _instance;
    public static AuthentificationAnimator instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AuthentificationAnimator>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            initialize();
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    #endregion


    private void initialize()
    {
        MainCanvas.alpha = 0.0f;
        MainCanvas.interactable = false;
        MainCanvas.blocksRaycasts = false; 

        LoginPanel.alpha = 0.0f;
        LoginPanel.interactable = false;
        LoginPanel.blocksRaycasts = false; 

        loader.alpha = 0.0f;
        loader.interactable = false; 
        loader.blocksRaycasts = false;

        Signup.alpha = 0.0f;
        Signup.interactable = false; 
        Signup.blocksRaycasts = false;

        ForgotPassword.alpha = 0.0f;
        ForgotPassword.interactable = false; 
        ForgotPassword.blocksRaycasts = false;

        StatusPanel.alpha = 0.0f;
        StatusPanel.interactable = false;
        StatusPanel.blocksRaycasts = false;

        /*
        ConsentForm.alpha = 0.0f;
        ConsentForm.interactable = false;
        ConsentForm.blocksRaycasts = false; 
        */

        UserProfile.alpha = 0.0f;
        UserProfile.interactable = false;
        UserProfile.blocksRaycasts = false; 

        currentstate = Auth.AuthStates.Default;
    }

    //controls the loader independently of other screens.
    public void SetLoader(bool visible)
    {
        float target = 0.0f;
        if(visible)
        {
            target = 1.0f;
        }
        DOTween.To(()=> loader.alpha, x => loader.alpha = x, target, 0.8f);  
    }

    public void ShowScreen(Auth.AuthStates state)
    {
        print("Showscreen");
        print("currentstate: " + currentstate);
        print("newstate: " + state);

        OpenCanvas();

        if (currentstate == state)
        {
            return;
        }
        else
        {
            currentstate = state;
            if (fadein != null)
            {
                fadeout = fadein;
            }

            switch (state)
            {
                case Auth.AuthStates.Login:
                    
                    fadein = LoginPanel;

                    break;
                case Auth.AuthStates.Signup:
                    fadein = Signup;
                    break;
                    /*
                case Auth.AuthStates.Consent:
                    fadein = ConsentForm;
                    break;
                    */
                case Auth.AuthStates.Forgot:
                    fadein = ForgotPassword;
                    break;
                case Auth.AuthStates.Default:
                    CloseAuth();
                    return;
                case Auth.AuthStates.Message :
                    print("case for message is working");
                    fadein = StatusPanel; 
                    break;
                case Auth.AuthStates.UserProfile :
                    fadein = UserProfile;
                    break;
            }
        }

        Sequence fadesequence = DOTween.Sequence();

        if (fadeout != null)
        { 
            fadesequence.Append(DOTween.To(() => fadeout.alpha, x => fadeout.alpha = x, 0.0f, fadespeed));
            fadeout.interactable = false;
            fadeout.blocksRaycasts = false; 
        }

        fadesequence.Append(DOTween.To(() => fadein.alpha, x => fadein.alpha = x, 1.0f, fadespeed));
        fadein.interactable = true;
        fadein.blocksRaycasts = true;

        fadesequence.Rewind(true);
        fadesequence.Play();
    }

    public void CloseAuth ()
    {
        fadeout = fadein;
        fadein = null;
        DOTween.To(() => fadeout.alpha, x => fadeout.alpha = x, 0.0f, fadespeed).OnComplete(ClearAuth);
    }

    private void ClearAuth()
    {
        fadein = null;
        MainCanvas.alpha = 0.0f;
        MainCanvas.interactable = false;
        MainCanvas.blocksRaycasts = false;
        currentstate = Auth.AuthStates.Default;
    }

    public void OpenAuth()
    {
        print("openauth");
        ShowScreen(Auth.AuthStates.Login);
    }
    /*
    public void openConsent()
    {
        OpenCanvas();
        ShowScreen(Auth.AuthStates.Consent);
    }
    */

    public void OpenCanvas()
    {
        MainCanvas.alpha = 1.0f;
        MainCanvas.interactable = true;
        MainCanvas.blocksRaycasts = true; 
        print("opencanvas");
    }
}
