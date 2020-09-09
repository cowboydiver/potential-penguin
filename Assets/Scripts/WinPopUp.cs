
public class WinPopUp : PopUp {

    void OnEnable() {
        
        if (!GameManager.IsInitialized) return;
        root = transform.GetChild(0);
        root.gameObject.SetActive(false);
        GameManager.Inst.Win += BannerPopUpDelay;
        GameManager.Inst.ShowWinMessage += BannerPopUpNow;
    }

    void OnDisable() {
        if (!GameManager.IsInitialized) return;

        GameManager.Inst.Win -= BannerPopUpDelay;
        GameManager.Inst.ShowWinMessage -= BannerPopUpNow;
        root.gameObject.SetActive(false);
    }
}
