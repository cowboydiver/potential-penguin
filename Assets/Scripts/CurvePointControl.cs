#if UNITY_EDITOR
using UnityEngine;

public class CurvePointControl : MonoBehaviour
{
    public int objectNumber;
    public GameObject controlObject;
    public GameObject controlObject2;
    public CurvePointControl ParentAnchor;
    GUITexture texture;
    Color originalColor;

    CurvePointControl neighborControl
    {
        get { return (ParentAnchor != null ? (ParentAnchor.controlObject != null && ParentAnchor.controlObject != gameObject) ? ParentAnchor.controlObject.GetComponent<CurvePointControl>() : (ParentAnchor.controlObject2 != null && ParentAnchor.controlObject2 != gameObject) ? ParentAnchor.controlObject2.GetComponent<CurvePointControl>() : null : null); }
    }

    void Awake()
    {
        texture = GetComponent<GUITexture>();
        originalColor = texture.color;
    }

    void OnMouseOver()
    {
        if(neighborControl != null)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                texture.color = Color.green;
            }
            else
            {
                texture.color = originalColor;
            }
        }
    }

    void OnMouseExit()
    {
        texture.color = originalColor;
    }

    void OnMouseDrag()
    {
        transform.position = EditCurve.cam.ScreenToViewportPoint(Input.mousePosition);
        EditCurve.use.UpdateLine(objectNumber, Input.mousePosition, gameObject);

        if (neighborControl != null && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            Vector2 myPosition = EditCurve.cam.ViewportToScreenPoint(transform.position);
            Vector2 parentPosition = EditCurve.cam.ViewportToScreenPoint(ParentAnchor.transform.position);
            Vector2 neighborPosition = EditCurve.cam.ViewportToScreenPoint(neighborControl.transform.position);

            Vector2 dir = (myPosition - parentPosition).normalized;
            float distance = (myPosition - parentPosition).magnitude;
            Vector2 screenPosition = parentPosition - dir * distance;

            neighborControl.transform.position = EditCurve.cam.ScreenToViewportPoint(screenPosition);
            EditCurve.use.UpdateLine(neighborControl.objectNumber, screenPosition, gameObject);
        }
    }
}
#endif