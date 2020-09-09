using UnityEngine;
using Parse;
using System;
using System.Collections.Generic;
using System.Linq;

public class PenguinDataManager : Singleton<PenguinDataManager> {
    
    //Main-thread-only data
    string deviceID;
    int currentLevel = -1;
    public LevelPlayedData leveldata;

    private float starttime = 0.0f;
    private float finishtime = 0.0f;
    private float timespent = 0.0f;
    private DateTime starttimestamp;


    void Awake()
    {
        if (inst == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        deviceID = SystemInfo.deviceUniqueIdentifier;
        leveldata = new LevelPlayedData();
    }

    int GetLocalCompletedLevels() {
        Debug.Log("GetLocal completed levels"); 

        int finishedlevels = 0; 

        string playerid = "";
        if(UserProfile.instance.IsLoggedIn)
        {
            playerid = UserProfile.instance.ID.ToString();
        }
        else
        {
            playerid = "localuser-" + SystemInfo.deviceUniqueIdentifier;
        }

        Debug.Log("PlayerId: " + playerid);

        if(PlayerPrefs.HasKey(playerid))
        {
            Debug.Log("Player id is in there: " + playerid);
            finishedlevels = PlayerPrefs.GetInt(playerid);
            Debug.Log("Progress is: " + finishedlevels);
        }

        return finishedlevels;
    }
    
	public void StartedLevel(int levelIndex) 
    {
        if(leveldata == null)
        {
            leveldata = new LevelPlayedData();
        }

        if(currentLevel != levelIndex)
        {
            leveldata.level_id = levelIndex;
            currentLevel = levelIndex;
            starttime = Time.time;
            starttimestamp = new DateTime();
        }
    }

    public void CompletedLevel(int levelIndex) 
    {
        string playerid = "";
        finishtime = Time.time;
        timespent = finishtime - starttime;

        if (!UserProfile.instance.IsLoggedIn)
        {
            playerid = "localuser-" + deviceID;
            leveldata.user_id = playerid;
        }
        else
        {
            playerid = UserProfile.instance.ID.ToString();
            leveldata.user_id = playerid;
        }

           leveldata.level_id = currentLevel;
           leveldata.language = LocalizationManager.Inst.GetLanguage();
           leveldata.start_time_stamp = starttimestamp;
           leveldata.start_time = starttime;
           leveldata.finish_time = finishtime;
           leveldata.time_spent = timespent;
           leveldata.level_completed = true;

           SaveData(leveldata);



        if(PlayerPrefs.HasKey(playerid))
        {
            if (PlayerPrefs.GetInt(playerid) < currentLevel)
            {
                PlayerPrefs.SetInt(playerid, currentLevel);
                PlayerPrefs.Save();
            }
        }
        else
        {
            PlayerPrefs.SetInt(playerid, currentLevel);
            PlayerPrefs.Save();
        }
    }

    private void SaveData(LevelPlayedData data)
    {
        
        DataManager.Save(data)
        .Then(response =>
        {
            leveldata = new LevelPlayedData();

        }).Catch(error => {
            Debug.LogError(error);
        });

        if(DataManager.instance.IsOffline())
        {
            leveldata = new LevelPlayedData();
        }
    }

}
