using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuCanvas;

    [Header("VR Controller Input")]
    public InputActionProperty menuButtonAction; // e.g., X or Y button

    private bool isMenuVisible = false;
    private bool isGamePaused = false;

    void Update()
    {
        // Toggle with VR controller (X/Y)
        if (menuButtonAction.action.WasPressedThisFrame())
        {
            TogglePauseMenu();
        }

        // Toggle with ESC key
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        isMenuVisible = !isMenuVisible;
        pauseMenuCanvas.SetActive(isMenuVisible);

        // Optional: Pause/unpause the game
        if (isMenuVisible)
        {
            Time.timeScale = 0f; // Pause game
            isGamePaused = true;
        }
        else
        {
            Time.timeScale = 1f; // Resume game
            isGamePaused = false;
        }
    }

    public void ResumeGame()
    {
        isMenuVisible = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // In case quitting returns to menu
        Application.Quit();
    }
}
