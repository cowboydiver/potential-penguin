
public class LoosePopUp : PopUp {

    void OnEnable() {

        if (!GameManager.IsInitialized) return;
        root = transform.GetChild(0);
        root.gameObject.SetActive(false);
        GameManager.Inst.Lost += BannerPopUpNow;
    }

    void OnDisable() {
        if (!GameManager.IsInitialized) return;

        GameManager.Inst.Lost -= BannerPopUpNow;
        root.gameObject.SetActive(false);
    }
}
