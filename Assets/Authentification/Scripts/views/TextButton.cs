using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Button button;
    public Text txt;
    private EventTrigger trigger;
    public SAHStyles.ColorMode style;

    private Color normalcolor;
    private Color overcolor; 

    private void Awake()
    {
        List<Color> cols =  ColorSchemes.GetTextButtonColors();
        normalcolor = cols[0];
        overcolor = cols[1];
             
        txt.color = normalcolor;

        ColorBlock c = button.colors;
        Color col = new Color(1, 1, 1, 0);
        c.normalColor = col;
        c.highlightedColor = col; //;colors[2];
        c.disabledColor = col;
        c.pressedColor = col;// colors[2];
        button.colors = c;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        txt.color = overcolor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        txt.color = normalcolor;
    }
}
