using UnityEngine;

public class LoginPopup : MonoBehaviour
{
    int levelNumber;

    private void OnEnable()
    {
        if (!UserProfile.instance.IsLoggedIn)
        {
        }
            
    }

    public void SetLevelNumber(int levelNumber)
    {
        this.levelNumber = levelNumber;
    }

	public void LoginButtonOnClick()
    {
        if(!UserProfile.instance.IsLoggedIn)
        {
            AuthentificationAnimator.instance.OpenAuth();
        }
        else
        {
            UserProfile.instance.Logout();
        }
        /*
        LoginManager.instance.LogoutFromParse();
        GameManager.Inst.SetUserLoggedIn(false);
        */
        ClosePopup();
    }

    public void PlayButtonOnClick()
    {
        GameManager.Inst.LoadLevel(levelNumber);

        ClosePopup();
    }

    void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}