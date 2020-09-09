using UnityEngine;

public class TestSceneButton : MonoBehaviour
{
    public int SceneIndex = 0;

    public void SceneButtonOnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(string.Concat("Level", SceneIndex.ToString("000")), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}