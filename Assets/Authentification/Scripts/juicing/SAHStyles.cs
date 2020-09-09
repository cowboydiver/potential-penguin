using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAHStyles : MonoBehaviour {

    #region singleton stuff
    private static SAHStyles _instance;
    public static SAHStyles instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SAHStyles>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    #endregion

    public enum ColorMode
    {
        Orange,
        Green,
        Yellow,
        Blue,
        Red,
        Purple,
        transparent,
        Default
    }

    public ColorMode IconColor;
    public ColorMode MainButton;
    public ColorMode NavigationButton;
    public ColorMode CloseButton;
    public ColorMode CancelButton;



}
