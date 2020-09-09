using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))][ExecuteInEditMode]
public class SAHButtonStyler : MonoBehaviour {

    public Button button; 

    public SAHStyles.ColorMode style;

    private void Awake()
    {
        
        List <Color> colors = new List<Color>(3);

        colors = ColorSchemes.GetColors(style);
        ColorBlock c = button.colors;
        c.normalColor = colors[0];
        c.highlightedColor = new Color(0, 0, 0); //;colors[2];
        c.disabledColor = colors[1];
        c.pressedColor = new Color(0, 0, 0);// colors[2];
        button.colors = c; 
    }

}
