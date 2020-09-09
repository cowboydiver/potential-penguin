using UnityEngine;
using DG.Tweening;

public class PulseEffect : MonoBehaviour
{
    public float Duration = 1f;
    public float EndScale = 1.1f;

    Tweener tweener;
    Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        OnDisable();

        transform.localScale = originalScale;

        tweener = transform.DOScale(EndScale, Duration).SetEase(Ease.InOutSine);
        tweener.SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable()
    {
        if (tweener != null && !tweener.IsComplete())
        {
            tweener.Kill();
            tweener = null;
        }
    }
}