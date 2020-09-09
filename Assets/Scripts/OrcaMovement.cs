using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OrcaMovement : MonoBehaviour
{
    IEnumerator moveRoutine;
    Tweener tweener0;
    Tweener tweener1;

	void Start()
    {

        moveRoutine = MoveOrca(new Vector3(transform.position.x * -1, transform.position.y, transform.position.z), Random.Range(40, 60));
        StartCoroutine(moveRoutine);
        tweener0 = transform.DOMoveY(transform.position.y - 0.2f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetUpdate(false);
	}

    void OnDisable()
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        if(tweener0 != null && !tweener0.IsComplete())
        {
            tweener0.Kill();
        }

        if (tweener1 != null && !tweener1.IsComplete())
        {
            tweener1.Kill();
        }
    }

    IEnumerator MoveOrca(Vector3 startPos, float duration)
    {
        float newXPos = startPos.x * -1;
        transform.forward = new Vector3(newXPos, 0, 0);
        transform.position = startPos;

        tweener1 = transform.DOMoveX(newXPos, duration).SetEase(Ease.Linear).SetUpdate(false);

        yield return new WaitForSeconds(duration);

        moveRoutine = MoveOrca(new Vector3(newXPos, startPos.y, Random.Range(15, 30)), Random.Range(40, 60));
        StartCoroutine(moveRoutine);
    }
}