using UnityEngine;

public class LoseCondition : MonoBehaviour
{
    public LoseConditionType ConditionType;

    public bool ShootForce = true;
    public float ShootForceMultiplier = 1f;

    public penguinAnimations looseAnimation = penguinAnimations.drowning;

    Penguin penguin;
    PenguinGUI penguinGUI;

    void Awake()
    {
        penguin = FindObjectOfType<Penguin>();
        penguinGUI = penguin.GetComponent<PenguinGUI>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Constants.PenguinTag)
        {
            switch (ConditionType)
            {
                case LoseConditionType.EnterTrigger:
                case LoseConditionType.LessThanKineticEnergy:
                    GameManager.Inst.CheckLost();

                    if (GameManager.Inst.lost)
                    {
                        penguin.PlayAnimation(looseAnimation, true);

                        if(ShootForce)
                            penguin.ShootForce(penguin.ActualVelocity * ShootForceMultiplier);

                    }
                    else if (GameManager.Inst.won)
                    {
                        penguin.StopSlowly(0f);
                    }
                    break;
            }
        }
    }

    void LateUpdate()
    {
        if (GameManager.Inst == null || GameManager.Inst.won || GameManager.Inst.lost)
        {
            return;
        }

        switch (ConditionType)
        {
            case LoseConditionType.MaxForceLimit:
                UpdateMaxForceLimit();
                break;
        }
    }

    void UpdateMaxForceLimit()
    {
        if (penguin.Force > penguinGUI.ForceShadowValue)
        {
            GameManager.Inst.CheckLost();

            if (GameManager.Inst.lost) {
                penguin.PlayAnimation(looseAnimation, true);
                penguin.UpdateForceArrow(false);
            }
        }
    }
}