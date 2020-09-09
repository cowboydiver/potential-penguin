using UnityEngine;

/// <summary>
/// Class used for testing functions by input
/// </summary>
public class Tester : MonoBehaviour {


    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            if(LocalizationManager.Inst.GetLanguage() == "EN")
                LocalizationManager.Inst.SetLanguage("DA");
            else
                LocalizationManager.Inst.SetLanguage("EN");
        }
    }
}
