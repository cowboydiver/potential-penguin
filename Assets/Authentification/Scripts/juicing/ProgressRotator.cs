using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressRotator : MonoBehaviour {

    public Image img;
    float zrotation = 0; 

	// Update is called once per frame
	void Update () 
    {
        //zrotation = img.rectTransform.rotation.z + (10 * Time.deltaTime);
        img.rectTransform.Rotate(new Vector3(0,0,5));
        //print("z: " + img.rectTransform.rotation.z + " zrotation: " + zrotation);
	}
}
