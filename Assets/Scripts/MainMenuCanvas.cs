using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public GameObject LoginPopupObject;

    public void ShowLoginPopup(bool show, int levelNumber)
    {
        LoginPopupObject.GetComponent<LoginPopup>().SetLevelNumber(levelNumber);
        LoginPopupObject.SetActive(show);
    }
}