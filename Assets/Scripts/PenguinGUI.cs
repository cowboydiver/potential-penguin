using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PenguinGUI : MonoBehaviour
{
    const float MaxForceArrowLength = 3f;
    const float ForceArrowMinPercentageVisible = 0.05f;
    const float MaxForce = 1f;
    const float LerpToTargetSpeed = 40f;
    const float LerpToTargetTime = 0.1f;

    public Image kinEnergyImage;
    public RectTransform Tangent;
    public RectTransform ForceArrow;

    public bool ShowShadow = false;
    public RectTransform ShadowForceArrow;
    public float ForceShadowValue;

    Tweener forceTweener;

    void Start()
    {
        if(ShowShadow)
        {
            ShadowForceArrow.gameObject.SetActive(true);

            float percentage = Mathf.Clamp01(ForceShadowValue / MaxForce);
            ShadowForceArrow.sizeDelta = new Vector2(Mathf.Lerp(0f, MaxForceArrowLength, percentage), ShadowForceArrow.sizeDelta.y);
        }
    }

    public void UpdatePieGUI(float totalEnergy, float kinE)
    {
        //Debug.Log("kineE " + Mathf.InverseLerp(0, totalEnergy, kinE));
        kinEnergyImage.fillAmount = Mathf.InverseLerp(0, totalEnergy, kinE);
    }

    public void ShowTangent(bool show)
    {
        Tangent.gameObject.SetActive(show);
    }

    public void UpdateForceArrow(float force, bool leftDirection)
    {
        float percentage = Mathf.Clamp01(force / MaxForce);

        if (percentage < ForceArrowMinPercentageVisible)
        {
            if (ForceArrow.gameObject.activeSelf)
            {
                ForceArrow.gameObject.SetActive(false);
            }
        }
        else if(!ForceArrow.gameObject.activeSelf)
        {
            ForceArrow.gameObject.SetActive(true);
        }

        float lengthValue = Mathf.Lerp(0f, MaxForceArrowLength, percentage);
        //lengthValue = Mathf.Lerp(ForceArrow.sizeDelta.x, lengthValue, Time.deltaTime * LerpToTargetSpeed);

        if(forceTweener != null)
        {
            forceTweener.Kill();
        }
        forceTweener = DOTween.ToAxis(() => ForceArrow.sizeDelta, x => ForceArrow.sizeDelta = x, lengthValue, LerpToTargetTime, AxisConstraint.X);

        //ForceArrow.sizeDelta = new Vector2(lengthValue, ForceArrow.sizeDelta.y);
        ForceArrow.transform.localScale = new Vector3((leftDirection ? -1 : 1), ForceArrow.transform.localScale.y, ForceArrow.transform.localScale.z);

        ShadowForceArrow.transform.localScale = new Vector3((leftDirection ? -1 : 1), ShadowForceArrow.transform.localScale.y, ShadowForceArrow.transform.localScale.z);
    }
}