using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    public GameObject firstPanel;
    public GameObject secondPanel;
    public Button nextButton;
    public Button startGameButton;

    // Event to notify game has started
    public static event Action OnGameStarted;

    void Start()
    {
        if (firstPanel == null || secondPanel == null || nextButton == null || startGameButton == null)
        {
            Debug.LogWarning("TutorialManager: Missing UI references.");
            return;
        }

        // Initially show only the first panel
        firstPanel.SetActive(true);
        secondPanel.SetActive(false);

        // Pause game during tutorial
        Time.timeScale = 0f;

        // Hook up button events
        nextButton.onClick.AddListener(HandleNextPanel);
        startGameButton.onClick.AddListener(HandleStartGame);
    }

    void HandleNextPanel()
    {
        firstPanel.SetActive(false);
        secondPanel.SetActive(true);

        // Hide Next button since there are no more panels
        nextButton.gameObject.SetActive(false);
    }

    void HandleStartGame()
    {
        Debug.Log("Start button clicked!");

        secondPanel.SetActive(false); // Hide tutorial

        // Call GameManager to fully start the game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is NULL! Unpausing manually.");
            Time.timeScale = 1f; // fallback: unpause physics
        }

        OnGameStarted?.Invoke(); // Notify listeners
    }
}
