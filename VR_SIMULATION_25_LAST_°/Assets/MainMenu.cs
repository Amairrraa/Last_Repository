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
        Application.Quit();
        Debug.Log("Game Exited"); 
    }
}
