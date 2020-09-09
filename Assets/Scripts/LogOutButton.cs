using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LogOutButton : MonoBehaviour {

    public GameObject LoginTextObject;
    public GameObject LogoutTextObject;

    Button bt;

    void Start() {
        bt = gameObject.GetComponent<Button>();
        bt.onClick.RemoveAllListeners();
        bt.onClick.AddListener(delegate ()
        {
            //LoginManager.instance.LogoutFromParse();
            //GameManager.Inst.SetUserLoggedIn(false);
        });
    }

    void Update()
    {

        /*
        if(Parse.ParseUser.CurrentUser == null)
        {
            if(!LoginTextObject.activeSelf)
            {
                LoginTextObject.SetActive(true);
                LogoutTextObject.SetActive(false);
            }
        }
        else if(!LogoutTextObject.activeSelf)
        {
            LoginTextObject.SetActive(false);
            LogoutTextObject.SetActive(true);
        }
        */
    }
}
