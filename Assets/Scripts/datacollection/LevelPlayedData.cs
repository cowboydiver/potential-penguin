using System; 

[System.Serializable]
public class LevelPlayedData: Writable  {
    public string user_id;

    public string language;

    public int level_id;
    public DateTime start_time_stamp;
    public float time_spent;
    public float time_pause;
    public float start_time;
    public float finish_time;
    public int number_of_times_paused;
    public int number_of_times_question_button_pressed;
    public int number_of_times_level_reset;

    public bool level_completed;
        
}
