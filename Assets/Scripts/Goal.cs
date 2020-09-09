using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    #region Variables
    const float RangeCompletedWaitTime = 1f;

    public WinConditionType Condition;

    public ParticleSystem[] fireworks;
    public Image kineticGUIGoal;
    public float StartRange;
    public float EndRange;

    Landscape landscape;
    Penguin penguin;
    InGameGUI gui;
    BulletTimeTrigger bulletTimeTrigger;

    public bool kineticPercentageToWin
    {
        get { return penguin.KineticEnergy >= StartRange && penguin.KineticEnergy <= EndRange; }
    }

    float startTime;
    #endregion
    #region Mono
    void Awake()
    {
        landscape = FindObjectOfType<Landscape>();
        penguin = FindObjectOfType<Penguin>();
        gui = FindObjectOfType<InGameGUI>();
        bulletTimeTrigger = FindObjectOfType<BulletTimeTrigger>();
        startTime = 0f;
    }

    void Start()
    {
        if ((landscape != null && landscape.EditMode) || GameManager.Inst == null)
        {
            return;
        }
    }


    void OnEnable() {
        if (!GameManager.IsInitialized) return;

        kineticGUIGoal.fillAmount = EndRange;
        GameManager.Inst.Win += FireWorks;
    }

    void OnDisable() {
        if (!GameManager.IsInitialized) return;

        GameManager.Inst.Win -= FireWorks;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (GameManager.Inst.won || GameManager.Inst.lost)
        {
            return;
        }

        if (collider.tag == Constants.PenguinTag)
        {
            switch (Condition)
            {
                case WinConditionType.EnterTrigger:
                    Won();
                    break;
                case WinConditionType.TriggerKineticEnergyRange:
                    if ((penguin.KineticEnergy / penguin.TotalEnergy) <= EndRange)
                    {
                        Won();
                    } 
                    break;
                case WinConditionType.TriggerForceRange:
                    if (penguin.Force >= StartRange && penguin.Force <= EndRange)
                    {
                        Won();
                    }
                    break;
                case WinConditionType.TriggerLowerForceRange:
                    if (penguin.Force >= EndRange) 
                    {
                        Won();
                    }
                    break;
            }
        }
    }

    public bool ShowBulletTime(WinConditionType condition) {
        bool show = true;

        switch (condition) {
            case WinConditionType.EnterTrigger:
                break;
            case WinConditionType.PotentialEnergyRange:
                break;
            case WinConditionType.KineticEnergyRange:
                break;
            case WinConditionType.TriggerKineticEnergyRange:
                show = (penguin.KineticEnergy / penguin.TotalEnergy - EndRange * 0.05f) <= EndRange;
                break;
            case WinConditionType.TriggerLowerForceRange:

                break;
            default:
                break;
        }


        return show;
    }

    void LateUpdate()
    {
        if((landscape != null && landscape.EditMode) || GameManager.Inst == null || GameManager.Inst.won || GameManager.Inst.lost)
        {
            return;
        }

        switch (Condition)
        {
            case WinConditionType.PotentialEnergyRange:
                UpdatePotentialEnergyRange();
                break;
            case WinConditionType.KineticEnergyRange:
                UpdateKineticEnergyRange();
                break;
        }
    }
    #endregion
    #region Private Methods
    void UpdatePotentialEnergyRange()
    {
        CheckTimeBasedRange(penguin.PotentialEnergy);
    }

    void UpdateKineticEnergyRange()
    {
        CheckTimeBasedRange(penguin.KineticEnergy);
    }

    void CheckTimeBasedRange(float currentValue)
    {
        if (currentValue >= StartRange && currentValue <= EndRange)
        {
            if (startTime == 0f)
            {
                startTime = Time.time;
            }
            else if (Time.time - startTime >= RangeCompletedWaitTime)
            {
                Won();
            }

            gui.UpdateEnergyPercentageCompleted((Time.time - startTime) / RangeCompletedWaitTime);
        }
        else
        {
            startTime = 0f;
            gui.UpdateEnergyPercentageCompleted(-1);
        }
    }

    void Won()
    {
        GameManager.Inst.CheckWin();

    }

    public void FireWorks() {
        foreach (ParticleSystem ps in fireworks) {
            ps.Play();
        }
    }
    #endregion
}
