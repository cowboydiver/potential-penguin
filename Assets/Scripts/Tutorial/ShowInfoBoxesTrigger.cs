using UnityEngine;

public class ShowInfoBoxesTrigger : MonoBehaviour
{
    public InformationBoxChain InfoBoxChain;
    public bool HideInfoBoxesOnAwake = true;

    bool didTrigger = false;

    void Awake()
    {
        if (InfoBoxChain == null)
        {
            Debug.LogError("You need to add the info box chain object here!");
        }
        //If replaying level
        else if (GameManager.Inst != null && GameManager.Inst.HideInfoBoxes)
        {
            InfoBoxChain.gameObject.SetActive(true);
            Destroy(gameObject);
        }
        else if(HideInfoBoxesOnAwake)
        {
            InfoBoxChain.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!didTrigger)
        {
            didTrigger = true;
            InfoBoxChain.gameObject.SetActive(true);
        }
    }
}