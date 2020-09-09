using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameGUI : MonoBehaviour
{
    #region Variables
    const float MinScale = 0.1f;
    const float MaxScale = 1f;
    const float MaxVelocity = 5f;

    public GameObject[] ActiveChildren;

    public Text TotalEnergyText, EqualSignText, KineticEnergyText, PlusSignText, PotentialEnergyText;
    //public RectTransform TotalEnergyRect, EqualSignRect, KinecticEnergyRect, PlusSignRect, PotentialEnergyRect;
    public float MaxTotalEnergy = 5f;

    public bool ShowShadow;
    public Text TotalEnergyShadowText, EqualSignShadowText, KineticEnergyShadowText, PlusSignShadowText, PotentialEnergyShadowText;
    public float PotentialEnergyShadowValue;
    public float KineticEnergyShadowValue;

    public Text TotalForceText, ForceEqualSignText, ForceTangent0Text, ForcePotentialText, ForceTangent1Text;

    public Image VelocityBar;
    public Button MinusKineticButton;
    public Button PlusKineticButton;

    public Image EnergyPercentageCompletedImage;
    #endregion
    #region Mono
    void Awake()
    {
        if (!GameManager.IsInitialized) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < ActiveChildren.Length; i++)
        {
            ActiveChildren[i].SetActive(true);
        }

        if(ShowShadow)
        {
            float tE = PotentialEnergyShadowValue + KineticEnergyShadowValue;
            float totalScale = Mathf.Clamp01(tE / MaxTotalEnergy);
            float potentialScale = (tE > 0f ? Mathf.Clamp01(PotentialEnergyShadowValue / tE) * totalScale : 0f);
            float kineticScale = (tE > 0f ? Mathf.Clamp01(KineticEnergyShadowValue / tE) * totalScale : 0f);

            if (TotalEnergyShadowText != null) TotalEnergyShadowText.rectTransform.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, totalScale);
            if (PotentialEnergyShadowText != null) PotentialEnergyShadowText.rectTransform.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, potentialScale);
            if (KineticEnergyShadowText != null) KineticEnergyShadowText.rectTransform.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, kineticScale);

            EnableTextBoxes(TotalEnergyShadowText, EqualSignShadowText, PotentialEnergyShadowText, PlusSignShadowText, KineticEnergyShadowText);
            UpdateTextPositions(TotalEnergyShadowText, EqualSignShadowText, PotentialEnergyShadowText, PlusSignShadowText, KineticEnergyShadowText);
        }

        EnableTextBoxes(TotalEnergyText, EqualSignText, PotentialEnergyText, PlusSignText, KineticEnergyText);
    }
    #endregion
    #region Public Methods
    public void UpdateEnergyTextSizes(float tE, float pE, float kE)
    {
        float totalScale = Mathf.Clamp01(tE / MaxTotalEnergy);
        float potentialScale = (tE > 0f ? Mathf.Clamp01(pE / tE) * totalScale : 0f);
        float kineticScale = (tE > 0f ? Mathf.Clamp01(kE / tE) * totalScale : 0f);

        if (TotalEnergyText != null) TotalEnergyText.rectTransform.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, totalScale);
        if (PotentialEnergyText != null) PotentialEnergyText.rectTransform.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, potentialScale);
        if (KineticEnergyText != null) KineticEnergyText.rectTransform.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, kineticScale);

        UpdateTextPositions(TotalEnergyText, EqualSignText, PotentialEnergyText, PlusSignText, KineticEnergyText);
       // UpdateBoxPositions()
    }

    public void UpdateForceTextPositions()
    {
        UpdateTextPositions(TotalForceText, ForceEqualSignText, ForceTangent0Text, ForcePotentialText, ForceTangent1Text);
    }

    public void UpdateEnergyPercentageCompleted(float percentage)
    {
        if(percentage >= 0f && !EnergyPercentageCompletedImage.transform.parent.gameObject.activeSelf)
        {
            EnergyPercentageCompletedImage.transform.parent.gameObject.SetActive(true);
        }
        else if(percentage < 0f)
        {
            EnergyPercentageCompletedImage.transform.parent.gameObject.SetActive(false);
        }

        EnergyPercentageCompletedImage.fillAmount = Mathf.Clamp01(percentage);
    }

    public void UpdateVelocityBar(float velocity)
    {
        velocity = Mathf.Abs(velocity);
        VelocityBar.fillAmount = Mathf.Clamp01(velocity / MaxVelocity);
    }

    public void EnableKineticButtons(bool enabled)
    {
        MinusKineticButton.interactable = enabled;
        PlusKineticButton.interactable = enabled;
    }

    public void MenuButtonOnClick()
    {
        GameManager.Inst.GotoMainMenu();
    }

    public void RestartButtonOnClick()
    {
        GameManager.Inst.RestartLevel();
    }

    public void HelpButtonOnClick()
    {
        GameManager.Inst.RestartLevel(false);
    }

    public void NextLevel()
    {
        GameManager.Inst.NextLevel();
    }
    #endregion
    #region Private Methods
    void EnableTextBoxes(params Text[] textBoxes)
    {
        for (int i = 0; i < textBoxes.Length; i++)
        {
            if (textBoxes[i] == null) continue;

            textBoxes[i].gameObject.SetActive(true);
        }
    }

    void UpdateTextPositions(params Text[] textBoxes) {
        float currentX = textBoxes[0].rectTransform.anchoredPosition.x + textBoxes[0].rectTransform.sizeDelta.x * textBoxes[0].rectTransform.localScale.x;

        for (int i = 1; i < textBoxes.Length; i++) {
            if (textBoxes[i] == null || !textBoxes[i].gameObject.activeSelf) continue;

            textBoxes[i].rectTransform.anchoredPosition = new Vector2(currentX, textBoxes[i].rectTransform.anchoredPosition.y);
            currentX += textBoxes[i].rectTransform.sizeDelta.x * textBoxes[i].rectTransform.localScale.x;
        }
    }

    //void UpdateBoxPositions(params RectTransform[] rects) {
    //    float currentX = rects[0].anchoredPosition.x + rects[0].sizeDelta.x * rects[0].localScale.x;

    //    for (int i = 1; i < rects.Length; i++) {
    //        if (rects[i] == null || !rects[i].gameObject.activeSelf) continue;

    //        rects[i].anchoredPosition = new Vector2(currentX, rects[i].anchoredPosition.y);
    //        currentX += rects[i].sizeDelta.x * rects[i].localScale.x;
    //    }
    //}
    #endregion
}