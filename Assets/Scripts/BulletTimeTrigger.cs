using UnityEngine;

public class BulletTimeTrigger : MonoBehaviour {

    public Transform lookTarget;
    public Transform moveTarget;
    public float maxTime = 12f;
    public bool disableOnExit = true;

    private float currentTime = 0f;
    public bool bulletTimeActive = false;

    private Goal goal;

    private void Update() {
        if (bulletTimeActive) {
            if (currentTime < maxTime) {
                currentTime += Time.unscaledDeltaTime;
                //Debug.Log(currentTime);
            }
            else {
                EnableBulletTime(false);
                currentTime = 0f;
            }
        }
    }

    private void Start() {
        goal = FindObjectOfType<Goal>();
    }

    private void OnDisable() {
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == Constants.PenguinTag && !GameManager.Inst.won)
        {
            if (goal.ShowBulletTime(goal.Condition) == false) {
                return;
            }
            EnableBulletTime(true);
        }    
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == Constants.PenguinTag && disableOnExit && !GameManager.Inst.won)
        {
            EnableBulletTime(false);
        }
    }

    private void EnableBulletTime(bool enable) {
        if (enable && !bulletTimeActive) {
            if (!GameManager.Inst.won) {
                CameraController.Inst.ZoomInWithSlowMotion(moveTarget, lookTarget);
                bulletTimeActive = true;
            }
        }
        else if(!enable && bulletTimeActive) {
            bulletTimeActive = false;
            CameraController.Inst.ZoomOut();
        }
    }
}