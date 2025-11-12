using UnityEngine;

public class PauseManager: MonoBehaviour 
{
    public GameObject pauseScreen;

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (pauseScreen.activeSelf)
            {
                // We're paused, so unpause
                pauseScreen.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                // We're unpaused, so pause
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}
