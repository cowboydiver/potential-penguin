using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Penguin : MonoBehaviour {

    #region Variables
    const float RotateSpeed = 10f;

    public bool ShowForceArrow = false;

    public float nextXValue;

    //public float TotalEnergy { get { return kinE + potE; } }
    public float TotalEnergy { get { return totalEnergy; } }
    public float KineticEnergy { get { return TotalEnergy - PotentialEnergy; } }
    public float PotentialEnergy { get { return potE; } }

    public float Force { get { return force; } }

    public Vector2 ActualVelocity { get { return actualVelocity; } }

    public penguinAnimations looseAnimation = penguinAnimations.sad;

    float potE = 0;
    float speed = Mathf.PI;
    float gravitationalConstant = 1f;
    //float height = 0;
    float totalEnergy = 0;
    float mass = 1f;
    float velocity;
    float force;
    float y;
    bool stopCalculatingPos = false;

    float potentialLowestValue;
    bool hasWon;

    bool isStopping;
    float stoppingStartTime;
    float stoppingTotalTime;
    float stoppingStartVelocity;

    bool isShootingForce;

    Vector2 actualVelocity;
    Vector3 lastPosition;

    Animator animator;

    Vector2 currentPositionOnLandscape;
    PenguinGUI penguinGUI;
    InGameGUI inGameGUI;
    #endregion
    #region Mono
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
	
    void Start()
    {
        inGameGUI = FindObjectOfType<InGameGUI>();
        penguinGUI = GetComponent<PenguinGUI>();
    }

    public void HitWater (Vector3 hit) {
        stopCalculatingPos = true;
        transform.position = new Vector3(transform.position.x, hit.y - 0.5f, transform.position.z);
        transform.DORotate(new Vector3(0, 90, 0), 1f);
        transform.DOMoveY(hit.y - 0.6f, 2f).OnComplete(() => transform.DOMoveY(hit.y - 0.8f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic));

        GetComponent<Rigidbody2D>().simulated = false;

        //PlayAnimation(penguinAnimations.drowning, true);
    }

    #endregion
    #region Public Methods
    public void SetInitialEnergy(float height, float potentialLowestValue) {
        this.potentialLowestValue = potentialLowestValue;
        totalEnergy = CalculatePotE(height);

    }

    public void UpdateTotalEnergyWithPotential(float newY, float oldY) {
        totalEnergy += (newY - oldY) * mass * gravitationalConstant;
        CalculatePotE(newY);
    }

    public void UpdateTotalEnergyWithKinetic(bool right)
    {
        bool velocitySign = Mathf.Approximately(Mathf.Sign(velocity), 1f);
        
        bool increase = (Mathf.Approximately(velocity, 0f) || (velocitySign && right) || (!velocitySign && !right));
        
        totalEnergy += increase ? 0.1f : -0.1f;
        totalEnergy = Mathf.Clamp(totalEnergy, PotentialEnergy, Mathf.Infinity);
        
        if (Mathf.Approximately(velocity, 0f))
        {
            velocity = right ? 0.01f : -0.01f;
        }
        else if (!increase && Mathf.Approximately(totalEnergy, 0f))
        {
            velocity = 0f;
        }
    }

    public void PlayAnimation(penguinAnimations animation, bool play) {
        switch (animation) {
            case penguinAnimations.won:
                animator.SetBool("Won", play);
                if (play) {
                    hasWon = true;
                }
                break;
            case penguinAnimations.sad:
                animator.SetBool("Sad", play);
                break;
            case penguinAnimations.happy:
                animator.SetBool("Happy", play);
                break;
            case penguinAnimations.drowning:
                animator.SetBool("Drowning", play);
                break;
            default:
                break;
        }
    }

    public float CalculatePotE(float height)
    {
        height = height - potentialLowestValue;

        return potE = height * mass * gravitationalConstant;
    }

    public void UpdatePenguinPosition (Vector2 previousPointInList, Vector2 currentPosition, Vector2 nextPointInList)
    {
        if (stopCalculatingPos)
        {
            return;
        }

        float acceleration;
        lastPosition = transform.position;

        Vector2 direction = nextPointInList - previousPointInList;
        direction = (direction != Vector2.zero) ? direction.normalized : Vector2.right;
        transform.position = new Vector3(currentPosition.x, currentPosition.y, transform.position.z);
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * RotateSpeed);

        acceleration = (previousPointInList.y - nextPointInList.y) / (nextPointInList.x - previousPointInList.x);
        float deltaVelocity = Time.deltaTime * speed * acceleration;
        y = (currentPosition.x - previousPointInList.x) * (nextPointInList.y - previousPointInList.y) / (nextPointInList.x - previousPointInList.x) + previousPointInList.y;

        if (velocity != 0f)
        {
            if (TotalEnergy > CalculatePotE(y))
            {
                velocity = Mathf.Sign(velocity) * Mathf.Sqrt(Mathf.Abs(TotalEnergy - CalculatePotE(y)) / mass * 2f)*Mathf.Cos(Mathf.Abs(Mathf.Atan((nextPointInList.y - previousPointInList.y) / (nextPointInList.x - previousPointInList.x))));
            }
            else
            {
                velocity = velocity + deltaVelocity / 10f;
            }            
        }
        else
        {
            velocity = velocity + deltaVelocity / 10f;
        }

        //Used for stopping the penguin slowly after winning
        if(isStopping)
        {
            if(velocity == 0f || stoppingTotalTime == 0f)
            {
                stopCalculatingPos = true;
            }
            else
            {
                float stopPercentage = Mathf.Clamp01((Time.time - stoppingStartTime) / stoppingTotalTime);
                velocity = Mathf.Lerp(stoppingStartVelocity, 0f, stopPercentage);
            }
        }

        nextXValue = currentPosition.x + (Time.deltaTime * speed * velocity);

        if(!hasWon)
        {
            penguinGUI.UpdatePieGUI(TotalEnergy, KineticEnergy);
            inGameGUI.UpdateEnergyTextSizes(TotalEnergy, potE, KineticEnergy);
            inGameGUI.UpdateVelocityBar(velocity);
        }

        force = Mathf.Abs(acceleration);
        if (ShowForceArrow) penguinGUI.UpdateForceArrow(force, (previousPointInList.y < nextPointInList.y));

        actualVelocity = (transform.position - lastPosition) / Time.deltaTime;

        //Debug.Log("A " + acceleration + " | V " + velocity + " | Y " + currentPosition.y + " | E " + TotalEnergy + "| K " + KineticEnergy + " | P " + PotentialEnergy);
    }

    public void StopSlowly(float stopTime)
    {
        if(!isStopping || stoppingTotalTime > 0f)
        {
            isStopping = true;
            stoppingStartTime = Time.time;
            stoppingTotalTime = stopTime;
            stoppingStartVelocity = velocity;
        }
    }

    public void ShootForce(Vector2 force)
    {
        if(!isShootingForce)
        {
            isShootingForce = true;
            stopCalculatingPos = true;

            Rigidbody2D rB = GetComponent<Rigidbody2D>();
            //rB.simulated = false;
            rB.bodyType = RigidbodyType2D.Dynamic;
            rB.mass = 0.8f;
            rB.AddForce(force, ForceMode2D.Impulse);
            rB.AddTorque(force.magnitude * (force.x > 0f ? -1f : 1f), ForceMode2D.Force);
        }
    }

    public void UpdateForceArrow(bool update)
    {
        ShowForceArrow = update;
    }
    #endregion
}
