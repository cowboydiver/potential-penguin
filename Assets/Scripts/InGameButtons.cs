using UnityEngine;
using UnityEngine.UI;

public class InGameButtons : MonoBehaviour {

    public Button resumeButton;
    public Button pauseButton;

    bool isPaused = false;

    void OnEnable() {
        if (!GameManager.IsInitialized) return;

        GameManager.Inst.Win += HideOnWin;
        GameManager.Inst.theEnablePauseButton += SetPauseButton;

        resumeButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();

        resumeButton.onClick.AddListener(delegate () { TogglePause(); });
        pauseButton.onClick.AddListener(delegate () { TogglePause(); });
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);

    }

    void OnDisable() {
        if (!GameManager.IsInitialized) return;

        GameManager.Inst.Win -= HideOnWin;
        GameManager.Inst.theEnablePauseButton -= SetPauseButton;

    }

    void HideOnWin() {
        gameObject.SetActive(false);
    }

    public void TogglePause() {
        isPaused = !isPaused;
        GameManager.Inst.Pause(isPaused);
        resumeButton.gameObject.SetActive(isPaused);
        pauseButton.gameObject.SetActive(!isPaused);
    }

    void SetPauseButton(bool enable) {
        pauseButton.interactable = enable;
    }
}
