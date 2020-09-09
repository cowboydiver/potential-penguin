using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InformationBoxChain : MonoBehaviour
{
    //public LineRenderer ln;
    //public Transform target;
    public InformationBox[] InfoBoxes;
    //public Transform InfoSlides;
    
    //public Button NextButton;
    public Sprite NextSprite;
    public Sprite CloseSprite;

    int infoSlidesCount;
    int activeSlide;

    RectTransform infoBoxTransform;

    //Vector3 endPosition;
    //public Vector3 lnEndPosition
    //{
    //    get { return endPosition; }
    //    set
    //    {
    //        endPosition = value;
    //        ln.SetPosition(1, endPosition);
    //    }
    //}

    void Start()
    {
        if (GameManager.Inst == null)
            return;

        if(GameManager.Inst.HideInfoBoxes)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Inst.Pause(true, false);

        infoSlidesCount = InfoBoxes.Length;

        if(infoSlidesCount == 0)
        {
            Debug.Log("No info boxes added to info box chain!");
        }

        for (int i = 0; i < infoSlidesCount; i++)
        {
            InfoBoxes[i].gameObject.SetActive(false);
        }

        ActiveSlide(0);
    }

    public void NextButtonOnClick()
    {
        activeSlide++;

        if (activeSlide >= infoSlidesCount)
        {
            // Close the last slide and continue the game. Set time back to normal
            InfoBoxes[activeSlide - 1].Hide();
            activeSlide = 0;
            GameManager.Inst.Pause(false, true);
            gameObject.SetActive(false);
        }
        else
        {
            if (activeSlide >= infoSlidesCount - 1)
            {
                InfoBoxes[activeSlide].SetButtonSprite(CloseSprite);
                // Change button to Continue Game
            }
            ActiveSlide(activeSlide);
        }
    }

    void ActiveSlide(int slideNumber)
    {
        Transform newSlide;
        if (slideNumber > 0)
        {
            InfoBoxes[slideNumber - 1].gameObject.SetActive(false);
            InfoBoxes[slideNumber - 1].Hide();
        }

        InfoBoxes[slideNumber].SetButtonSprite(((slideNumber >= infoSlidesCount - 1) ? CloseSprite : NextSprite));

        newSlide = InfoBoxes[slideNumber].transform;
        newSlide.gameObject.SetActive(true);
        infoBoxTransform = newSlide.GetComponent<RectTransform>();
        InfoBoxes[slideNumber].Show();

        activeSlide = slideNumber;

        //PointLinerender(newSlide.GetComponent<InformationBox>().target);
    }

    //void PointLinerender(Transform target)
    //{
    //    ln.SetPosition(0, infoBoxTransform.position);
    //    DOTween.To(() => lnEndPosition, x => lnEndPosition = x, target.position, 0.7f).SetEase(Ease.OutExpo);
    //}
}