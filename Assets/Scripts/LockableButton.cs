using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LockableButton : MonoBehaviour {

    public Button bt;
    public Image lockImage;
    public Text levelNumber;

    public int LevelIndex { get { return (int.Parse(levelNumber.text.Trim()) - 1); } }

    void Start() {
        Button bt = gameObject.GetComponent<Button>();
        bt.onClick.RemoveAllListeners();
        bt.onClick.AddListener(delegate () { GameManager.Inst.LoadLevel(LevelIndex); });
    }
    
    public void Lock() {
        if(LevelIndex > 0)
        {
            bt.interactable = false;
            lockImage.enabled = true;
            levelNumber.enabled = false;
        }
        else
        {
            Unlock();
        }
    }

    public void Unlock() {
        bt.interactable = true;
        lockImage.enabled = false;
        levelNumber.enabled = true;
    }

    public bool IsLocked() {
        return !bt.IsInteractable();
    }

}
