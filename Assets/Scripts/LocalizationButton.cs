using UnityEngine;
using UnityEngine.UI;

public class LocalizationButton : MonoBehaviour {

    public Languages lang = Languages.EN;

    public Image checkmark;

    private void OnEnable() {

        checkmark = GetComponentsInChildren<Image>(true)[1]; //exclude the parent
    }

    private void Start() {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate () { LocalizationManager.Inst.SetLanguage(lang.ToString()); });

    }

    public void SetCheckmark(bool enable) {
        checkmark.gameObject.SetActive(enable);
    }
}
