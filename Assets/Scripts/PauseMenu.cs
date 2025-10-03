using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Pannello del menu
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  // riparte il tempo
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  // ferma il tempo
        isPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // resetta il tempo
        SceneManager.LoadScene("MainMenu"); // sostituisci con il nome della tua scena del menu
    }
}
