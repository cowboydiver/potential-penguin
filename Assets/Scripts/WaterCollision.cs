using UnityEngine;
using DG.Tweening;

public class WaterCollision : MonoBehaviour {

    // Use this for initialization
    public Transform parentWater;

	void Start () {
        parentWater.transform.DORotate(new Vector3(0.03f, 0, 0), 2).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo).SetUpdate(false);
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.transform.tag == Constants.PenguinTag) {
            collision.transform.GetComponent<Penguin>().HitWater(transform.position);
        }
    }
}
