using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuitButton : MonoBehaviour {

    Button bt;

    void Start() {
        bt = gameObject.GetComponent<Button>();
        bt.onClick.RemoveAllListeners();
        bt.onClick.AddListener(delegate ()
        {
            GameManager.Inst.Quit();
        });
    }
}
