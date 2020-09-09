using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
//using Parse;
using System;

public class GameManager : Singleton<GameManager> {

    public bool debugging = false;
    public Text winText;
    public bool won = false;
    public bool lost = false;
    public LockableButton[] levelButtons;

    public GameObject UserAndParseObject;
    public LoadingScreen LoadingScreen;

    public Text debug;

    private float totalpausetime = 0.0f;
    private float startpause = 0.0f;
    private float stoppause = 0.0f;

    public static bool IsInitialized { get; private set; }

    public bool IsInputLocked { get { return inputLocked || won || lost; } }

    private bool isUserLoggedIn = false;
    private float unPausedTimeScale = 1f;
    private int currentlevel = 0;
    private int currentloading = -1;
    private bool isReplaying = false;

    Action logincallback;

    public bool HideInfoBoxes { get; private set; }

    bool inputLocked;

    bool didShowLoginPopup;

    InGameGUI ingameGUI;

    public delegate void WinFunction();
    public WinFunction Win;

    public delegate void ShowWinPopUp();
    public ShowWinPopUp ShowWinMessage;

    public delegate void LooseFunction();
    public LooseFunction Lost;

    public delegate void EnablePauseButton(bool enable);
    public EnablePauseButton theEnablePauseButton;

    Tweener pauseTweener;

    void Awake()
    {
        if(inst == null)
        {
            DontDestroyOnLoad(gameObject);

            Inst.ToString();
            IsInitialized = true;

            SceneManager.sceneLoaded += OnLevelLoaded;
            logincallback = UnlockLevels;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Win += WinStuff;
        Lost += LooseStuff;
        Auth.instance.updatemainmenu += logincallback;
        if (SceneManager.GetActiveScene().name == Constants.MainMenu)
        init();

    }

    private void init()
    {
        Debug.Log("Init called");
        updateLevelButtons();
        UnlockLevels();
    }

    private void Update() {
        if(won && Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("Check");
            ShowWinMessage();
        }
    }

    public void updateLevelButtons ()
    {
        GameObject buttons = GameObject.Find("Levels");
        levelButtons = buttons.GetComponentsInChildren<LockableButton>();
    }

    void OnDestroy()
    {
        if(inst == this) IsInitialized = false;
    }

    public void UnlockLevels()
    {
        Debug.Log("Unlocklevels");
        Debug.Log("buttons in list: " + levelButtons.Length);
        //int i = 0;
        int opened = GetLocalCompletedLevels() + 1;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i <= opened)
            {
                levelButtons[i].Unlock();
            }
            else
            {
                levelButtons[i].Lock();
            }
        }
    }

    public void CheckWin() {
        if (!won && !lost) {
            Win();
        }
    }

    public void CheckLost()
    {
        if (!lost && !won)
        {
            Lost();
        }
    }

    public void Pause(bool isPause, bool enablePauseButton) {

        if (theEnablePauseButton != null) {
            theEnablePauseButton(enablePauseButton);
        }

        SetInputLocked(isPause);

        if(pauseTweener != null)
        {
            pauseTweener.Kill();
        }

        if (isPause)
        {
            startpause = Time.time;
            PenguinDataManager.Inst.leveldata.number_of_times_paused++;
            unPausedTimeScale = 1;// Time.timeScale;
            pauseTweener = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 0.3f).SetEase(Ease.OutQuad);
        }
        else
        {
            stoppause = Time.time;
            float timepaused = stoppause - startpause;
            PenguinDataManager.Inst.leveldata.time_pause += timepaused;
            pauseTweener = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, unPausedTimeScale, 0.3f).SetEase(Ease.OutQuad);
        }
    }

    public void Pause(bool isPause) 
    {
        Pause(isPause, true);
    }

    int GetLocalCompletedLevels()
    {

        int finishedlevels = 0;

        string playerid = "";
        if (UserProfile.instance.IsLoggedIn)
        {
            playerid = UserProfile.instance.ID.ToString();
        }
        else
        {
            playerid = "localuser-" + SystemInfo.deviceUniqueIdentifier;
        }

        //Debug.Log("PlayerId: " + playerid);

        if (PlayerPrefs.HasKey(playerid))
        {

            finishedlevels = PlayerPrefs.GetInt(playerid);
            //Debug.Log("Progress is: " + finishedlevels);
        }

        return finishedlevels;
    }


    public void UpdatePlayerProgress() {
        UnlockLevels();
    }

    public void GotoMainMenu() {
        Pause(false);
        SceneManager.LoadScene(Constants.MainMenu);
        currentlevel = 0;
    }

    public void LoadLastUnlockedLevel() {

        int lastUnlockedLevel = 0;

        for (int i = 0; i < levelButtons.Length; i++) {
            if (!levelButtons[i].IsLocked()) {
                lastUnlockedLevel = i;
            }
            else {
                break;
            }
        }
        if(currentloading != lastUnlockedLevel)
        {
            LoadLevel(lastUnlockedLevel);
        }
    }

    public void LoadLevel(int levelNumber)
    {

        currentloading = levelNumber;

        LoadingScreen.Show(true);
        HideInfoBoxes = false;

        Pause(false);
   
        currentlevel = levelNumber;

        PenguinDataManager.Inst.StartedLevel(levelNumber);
        SceneManager.LoadScene(string.Concat("Level", levelNumber.ToString("000")));

    }

    public void RestartLevel(bool hideInfoBoxes = true) {
        if(hideInfoBoxes)
        {
            //Player is resetting the level
            PenguinDataManager.Inst.leveldata.number_of_times_level_reset++;
        }
        else
        {
            //Player is using help button
            PenguinDataManager.Inst.leveldata.number_of_times_question_button_pressed++;
        }
        won = false;
        LoadLevel(currentlevel);
        HideInfoBoxes = hideInfoBoxes;
    }

    public void NextLevel() {
        if (currentlevel + 1 < levelButtons.Length)
            LoadLevel(currentlevel + 1);
        else
            GotoMainMenu();
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;

        if(ingameGUI != null)
            ingameGUI.EnableKineticButtons(!IsInputLocked);
    }

    void WinStuff() {
        if (won)
            return;
        won = true;

        SetInputLocked(true);

        if (winText != null)
            winText.rectTransform.DOScale(1f, 1.2f).SetEase(Ease.OutElastic);

        if (!debugging)
            PenguinDataManager.Inst.CompletedLevel(int.Parse(SceneManager.GetActiveScene().name.Substring(5)));

        //For animation of penguin
        GameObject.FindGameObjectWithTag(Constants.PenguinTag).GetComponent<Penguin>().PlayAnimation(penguinAnimations.won, true);
    }

    void LooseStuff()
    {
        lost = true;
        SetInputLocked(true);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void OnLevelLoaded(Scene arg0, LoadSceneMode arg1)
    {
        won = false;
        lost = false;
        inputLocked = false;

        DOTween.KillAll();


        if (arg0.name == Constants.MainMenu) {
            init();
            UpdatePlayerProgress();
            //updateLevelButtons();

            ingameGUI = null;
        }
        else
        {
            ingameGUI = FindObjectOfType<InGameGUI>();
            LoadingScreen.Show(false);
        }
    }
}