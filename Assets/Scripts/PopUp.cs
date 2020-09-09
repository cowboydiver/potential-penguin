using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class PopUp : MonoBehaviour {

    public float duration = 0.8f;
    public float delay = 0.3f;
    public Vector3 moveFromDeltaPos = new Vector3(0, 50f, 0);
    public Ease easing = Ease.OutBack;
    public float popUpDelay = 3f;
    bool popShown = false;
    public Transform root;

    public void BannerPopUpDelay() {
        StartCoroutine(PopBanner(popUpDelay));
    }

    public void BannerPopUpNow() {
        float popUpDelay = 0f;
        StartCoroutine(PopBanner(popUpDelay));
    }

    public IEnumerator PopBanner(float delayPopUP) {
        yield return new WaitForSeconds(delayPopUP);
        if (popShown) {
            yield break;
        }

        root.gameObject.SetActive(true);
        Graphic[] graphics = GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++) {
            Graphic g = graphics[i];
            Vector3 moveToPos = g.rectTransform.localPosition;
            g.rectTransform.localPosition += moveFromDeltaPos;
            g.color = new Color(1, 1, 1, 0);
            g.DOFade(1, duration).SetDelay(delay * i);
            g.rectTransform.DOLocalMove(moveToPos, duration).SetDelay(delay * i).SetEase(easing);
        }
        popShown = true;
    }

}
