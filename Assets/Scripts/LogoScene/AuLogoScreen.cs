using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AuLogoScreen : MonoBehaviour
{
    CanvasGroup cGroup;
    float timeStart = -0.5f;
    bool skip = false;
    public float timeOnScreen = 2;
    private static bool alreadyShown = false;

    void Start()
    {
        GetComponent<Canvas>().enabled = true;
        cGroup = GetComponent<CanvasGroup>();

        DontDestroyOnLoad(gameObject);

        if (!alreadyShown)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                SceneManager.LoadScene(Constants.MainMenu);
            }
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == Constants.MainMenu) {
			if (cGroup.alpha > 0 && Input.GetMouseButtonDown(0)) {
				skip = true;
			}
            if (Time.renderedFrameCount < 0) return;
			if (timeStart == -0.5f) timeStart = Time.time;
			if (skip || timeStart + timeOnScreen - Time.time <= 0) {
				cGroup.blocksRaycasts = false;
                cGroup.alpha -= Time.deltaTime;
                alreadyShown = true;
                if (cGroup.alpha <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}