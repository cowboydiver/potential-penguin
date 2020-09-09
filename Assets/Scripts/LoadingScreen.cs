using UnityEngine;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    static LoadingScreen inst;

    CanvasGroup canvasGroup;

    void Awake()
    {
        if(inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

            canvasGroup = GetComponent<CanvasGroup>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show(bool show)
    {    
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        if (!show)
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}