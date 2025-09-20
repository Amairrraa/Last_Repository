using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;

    public void Play()
    {
        SceneManager.LoadScene("GAME");
    }

    public void Settings()
    {
        Debug.Log("Settings button clicked");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        // For testing in the editor
        Debug.Log("Game would exit now (Editor mode)");
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        // For Oculus Quest 2 (APK build)
        Debug.Log("Exiting game on Quest 2...");
        Application.Quit();
#else
        // For other platforms
        Application.Quit();
#endif
    }
}
