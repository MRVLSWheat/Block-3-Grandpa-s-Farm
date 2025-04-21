using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject player; // drag your Player prefab here
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        UnlockCursor(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
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
        Time.timeScale = 1f;
        SetPlayerControlsEnabled(true);
        UnlockCursor(false);
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        SetPlayerControlsEnabled(false);
        UnlockCursor(true);
        isPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void SetPlayerControlsEnabled(bool enabled)
    {
        if (player != null)
        {
            var input = player.GetComponent<PlayerInput>();
            if (input != null)
                input.enabled = enabled;

            var starterInput = player.GetComponent<StarterAssets.StarterAssetsInputs>();
            if (starterInput != null)
                starterInput.enabled = enabled;

            var controller = player.GetComponent<StarterAssets.FirstPersonController>();
            if (controller != null)
                controller.enabled = enabled;
        }
    }

    private void UnlockCursor(bool unlock)
    {
        Cursor.visible = unlock;
        Cursor.lockState = unlock ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
