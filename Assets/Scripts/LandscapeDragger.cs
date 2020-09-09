using UnityEngine;
using DG.Tweening;

public class LandscapeDragger : MonoBehaviour
{
    #region Variables
    const float DragSpeed = 0.01f;

    public float MinYOffset = 0f;
    public float MaxYOffset = 1f;
    public int[] DragLandscapePointIndexes;

    Landscape landscape;
    Penguin penguin;

    new SpriteRenderer renderer;
    Color originalColor;

    new Transform transform;
    Vector3 position;
    Vector3 initialPosition;

    bool isDragging;
    bool isPaused;

    Vector3[] initialControlPoints;
    float[] controlPointYValues;

    Vector2 initialMousePosition;
    Vector3 initialDragPosition;

    Tweener pulseTweener;
    #endregion
    #region Mono
    void Start()
    {
        transform = GetComponent<Transform>();
        landscape = FindObjectOfType<Landscape>();
        penguin = FindObjectOfType<Penguin>();

        renderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = renderer.color;

        if(landscape.EditMode)
        {
            DestroyImmediate(gameObject);
            return;
        }

        pulseTweener = transform.GetChild(0).DOScale(transform.GetChild(0).localScale.x * 1.1f, 1f).SetEase(Ease.InOutSine);
        pulseTweener.SetLoops(-1, LoopType.Yoyo);

        if(DragLandscapePointIndexes.Length > 0)
        {
            position = Vector3.zero;
            initialControlPoints = landscape.GetLineControlPoints(DragLandscapePointIndexes);
            controlPointYValues = new float[initialControlPoints.Length];

            for (int i = 0; i < initialControlPoints.Length; i++)
            {
                position += initialControlPoints[i];
                controlPointYValues[i] = initialControlPoints[i].y;
            }
            position /= initialControlPoints.Length;
            position.z = position.z - 0.1f;

            transform.localPosition = initialPosition = position;
        }
        else
        {
            Debug.LogError("Dragger needs to control at least 1 control point!");
        }
    }

    void Update()
    {
        if(pulseTweener != null && GameManager.IsInitialized)
        {
            if (GameManager.Inst.IsInputLocked)
            {
                if (pulseTweener.IsPlaying())
                {
                    isPaused = true;
                    pulseTweener.Pause();
                }
            }
            else if (!pulseTweener.IsPlaying())
            {
                isPaused = false;
                pulseTweener.Play();
            }
        }
    }

    void OnDestroy()
    {
        if(pulseTweener != null && !pulseTweener.IsComplete())
        {
            pulseTweener.Kill();
            pulseTweener = null;
        }
    }
    #endregion
    #region Public Methods

    #endregion
    #region Private Methods
    void OnMouseDown()
    {
        isDragging = true;
        OnMouseEnter();

        initialMousePosition = Input.mousePosition;
        initialDragPosition = position;
    }

    void OnMouseUp()
    {
        isDragging = false;
        OnMouseExit();
    }

    void OnMouseDrag()
    {
        float yDiff = (Input.mousePosition.y - initialMousePosition.y) * DragSpeed;
        SetPosition(initialDragPosition.y + yDiff);
    }

    void OnMouseEnter()
    {
        if(!isPaused)
        {
            renderer.color = Color.Lerp(originalColor, Color.white, 0.1f);
        }
    }

    void OnMouseExit()
    {
        if(!isDragging)
        {
            renderer.color = originalColor;
        }
    }

    void SetPosition(float y)
    {
        //Don't change landscape when game over
        if(GameManager.Inst.IsInputLocked)
        {
            return;
        }

        Vector2 oldPengionPos = penguin.transform.position;

        position.y = Mathf.Clamp(y, initialPosition.y + MinYOffset, initialPosition.y + MaxYOffset);
        transform.localPosition = position;

        for (int i = 0; i < controlPointYValues.Length; i++)
        {
            controlPointYValues[i] = position.y - (initialPosition.y - initialControlPoints[i].y);
        }
        landscape.MoveLineControlPoints(controlPointYValues, DragLandscapePointIndexes);
        landscape.UpdateEnergyInSystem(oldPengionPos);
    }
    #endregion
}