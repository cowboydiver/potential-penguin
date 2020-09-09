using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AuthScreenScaler : MonoBehaviour {

    public static int screenwidth;
    public static int screenheight; 

    private void Awake()
    {
        RectTransform rect = this.GetComponent<RectTransform>();

        screenwidth = Screen.width;
        screenheight = Screen.height;

        rect.sizeDelta = new Vector2(screenwidth, screenheight);

    }

}
