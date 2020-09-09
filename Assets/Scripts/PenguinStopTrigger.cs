using UnityEngine;

public class PenguinStopTrigger : MonoBehaviour
{
    public float StopTime = 0.5f;

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.tag == Constants.PenguinTag && (GameManager.Inst.won || GameManager.Inst.lost))
        {
            collider.GetComponent<Penguin>().StopSlowly(StopTime);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == Constants.PenguinTag && (GameManager.Inst.won || GameManager.Inst.lost))
        {
            collider.GetComponent<Penguin>().StopSlowly(0f);
        }
    }
}