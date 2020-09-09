using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InformationBox : MonoBehaviour
{
    InfoBoxLogic logic;
    public Button bt;

    void Awake()
    {
        logic = GetComponent<InfoBoxLogic>();

        if (bt != null)
        {
            //bt = gameObject.GetComponent<Button>();
            bt.onClick.RemoveAllListeners();
            bt.onClick.AddListener(ButtonOnClick);
        }

    }

  private void OnEnable()
  {
    
  }

  public void Show()
    {
        if (logic != null) logic.InfoBoxShown();
    }

    public void Hide()
    {
        if (logic != null) logic.InfoBoxHidden();
    }

    public void ButtonOnClick()
    {
        //bt.gameObject.SetActive(false);
        GetComponentInParent<InformationBoxChain>().NextButtonOnClick();
    }

    public void SetButtonSprite(Sprite buttonSprite)
    {
        GetComponentInChildren<Button>().GetComponent<Image>().sprite = buttonSprite;
    }
}