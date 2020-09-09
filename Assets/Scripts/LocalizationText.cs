using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class LocalizationText :  MonoBehaviour {

    public string textKey;
    Text text;

    void OnEnable()
    {
        if(LocalizationManager.Inst == null)
        {
            return;
        }

        text = GetComponent<Text>();

        UpdateText();

        LocalizationManager.Inst.UpdateTextObjects += UpdateText;

    }

    void UpdateText() {

        string s = LocalizationManager.Inst.GetText(textKey);

        text.text = s.Replace("\\n", "\n");

    }

    void OnDisable() {

        if(LocalizationManager.Inst != null)
            LocalizationManager.Inst.UpdateTextObjects -= UpdateText;
    }
}
