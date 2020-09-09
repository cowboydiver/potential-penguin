using UnityEngine;

public class LanguageButtonGroup : MonoBehaviour {

    LocalizationButton[] languageButtons;

    private void Start() {
        languageButtons = GetComponentsInChildren<LocalizationButton>();
        RefreshSelectedButton();
        LocalizationManager.Inst.UpdateTextObjects += RefreshSelectedButton;
    }

    private void OnDisable() {
        if (LocalizationManager.Inst == null) return;

        LocalizationManager.Inst.UpdateTextObjects -= RefreshSelectedButton;
    }

    public void RefreshSelectedButton() {

        string currentLang = LocalizationManager.Inst.GetLanguage();

        foreach (LocalizationButton bt in languageButtons) {
            if (bt.lang.ToString() == currentLang)
                bt.SetCheckmark(true);
            else
                bt.SetCheckmark(false);
        }
    }
}
