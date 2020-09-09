using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(Image))]
public class SAHLogo : MonoBehaviour {

    public SAHStyles.ColorMode color;
    [SerializeField] private Image logo;

    private void Awake()
  {
        logo.color = ColorSchemes.GetColor(SAHStyles.instance.IconColor);
  }
}
