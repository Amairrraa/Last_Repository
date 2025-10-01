using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Over Panel Settings")]
    public TextMeshProUGUI finalScoreText;

    [Header("Live Player Score Display")]
    public TextMeshProUGUI playerScoreText; // NEW VARIABLE

    [Header("Scoring System")]
    public int totalBarrierScore = 0;
    public int freshwaterCount = 0;
    public int marineCount = 0;
    public int terrestrialCount = 0;
    [SerializeField] private float _currentMultiplier = 1.0f;

    [Header("Protection Settings")]
    public float baseMultiplier = 1.0f;
    public float ProtectionMultiplier => _currentMultiplier;
    private ProtectionLevel _currentProtectionLevel = ProtectionLevel.None;

    [Header("Game Flow")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI timerText;
    public float gameDuration = 150f;
    public GameObject ropesParent;
    public float ropeShowTime = 30f;
    public GameObject gameOverPanel;

    [Header("Extra Objects After Ropes")]
    [SerializeField] private GameObject[] extraObjects = new GameObject[6];
    public float extraObjectsShowTime = 20f;

    private float timer;
    private bool gameActive = false;
    private bool ropesShown = false;
    private bool extrasShown = false;

    public enum ProtectionLevel
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4
    }

    private void Awake() => InitializeSingleton();
    void Start() => InitializeGame();

    void Update()
    {
        if (!gameActive) return;

        UpdateTimer();
        CheckRopeVisibility();
        CheckExtraObjectsVisibility();
        UpdatePlayerScoreDisplay(); // CONTINUOUS UPDATE
    }

    #region Core Game Functions
    private void InitializeSingleton()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void InitializeGame()
    {
        timer = gameDuration;
        Time.timeScale = 0f;
        SetUIState(tutorialPanel, true);
        SetUIState(ropesParent, false);
        SetUIState(gameOverPanel, false);
        _currentMultiplier = baseMultiplier;

        foreach (var obj in extraObjects)
            SetUIState(obj, false);

        UpdatePlayerScoreDisplay(); // INITIALIZE TEXT
    }

    private void UpdateTimer()
    {
        timer -= Time.deltaTime;
        timerText.text = $"Time: {Mathf.FloorToInt(timer / 60f):00}:{Mathf.FloorToInt(timer % 60f):00}";
        if (timer <= 0f) EndGame();
    }

    private void CheckRopeVisibility()
    {
        if (!ropesShown && timer <= ropeShowTime)
        {
            ropesShown = true;
            SetUIState(ropesParent, true);
            SetProtectionLevel(_currentProtectionLevel);
            Debug.Log($"[ROPE] Ropes visible. Protection Level: {_currentProtectionLevel}");
        }
    }

    private void CheckExtraObjectsVisibility()
    {
        if (!extrasShown && timer <= extraObjectsShowTime)
        {
            extrasShown = true;
            foreach (var obj in extraObjects)
                SetUIState(obj, true);
            Debug.Log("[EXTRA OBJECTS] Activated after extraObjectsShowTime.");
        }
    }
    #endregion

    #region Protection Multiplier System
    public void SetProtectionLevel(ProtectionLevel level)
    {
        _currentProtectionLevel = level;
        if (ropesShown)
        {
            _currentMultiplier = baseMultiplier + GetIncrementForLevel(level);
            Debug.Log($"[PROTECTION] Level {level} → Multiplier: {_currentMultiplier:F3}");
        }
    }

    private float GetIncrementForLevel(ProtectionLevel level)
    {
        return level switch
        {
            ProtectionLevel.None => 0.000f,
            ProtectionLevel.Level1 => 0.125f,
            ProtectionLevel.Level2 => 0.250f,
            ProtectionLevel.Level3 => 0.375f,
            ProtectionLevel.Level4 => 0.500f,
            _ => 0f
        };
    }
    #endregion

    #region Scoring System
    public int TotalBiodiversityScore => freshwaterCount + marineCount + terrestrialCount;

    public float GetMatchScore()
    {
        float distributionFactor = CalculateDistributionFactor();
        float biodiversityComponent = TotalBiodiversityScore * distributionFactor;
        float finalScore = (totalBarrierScore + biodiversityComponent) * _currentMultiplier;

        return finalScore;
    }

    private float CalculateDistributionFactor()
    {
        float avg = TotalBiodiversityScore / 3f;
        float sigma = Mathf.Sqrt((Mathf.Pow(freshwaterCount - avg, 2) +
                                Mathf.Pow(marineCount - avg, 2) +
                                Mathf.Pow(terrestrialCount - avg, 2))
                                / 3f);

        return sigma switch
        {
            > 0f and <= 1f => 1.0f,
            > 1f and < 10f => 0.6f,
            _ => 0.5f
        };
    }

    private void UpdatePlayerScoreDisplay()
    {
        if (playerScoreText != null)
        {
            playerScoreText.text = $"Score: {GetMatchScore()}"; // NO ROUNDING
        }
    }
    #endregion

    #region Game State Management
    public void StartGame()
    {
        SetUIState(tutorialPanel, false);
        Time.timeScale = 1f;
        gameActive = true;
        Debug.Log("[GAME] Started!");
    }

    public void EndGame()
    {
        gameActive = false;
        SetUIState(ropesParent, false);
        float finalScore = GetMatchScore();

        if (finalScoreText != null)
            finalScoreText.text = finalScore.ToString(); // NO ROUNDING

        SetUIState(gameOverPanel, true);
        Time.timeScale = 0f;
    }

    public bool IsGameActive() => gameActive;
    #endregion

    #region Helper Methods
    private void SetUIState(GameObject element, bool state)
    {
        if (element != null) element.SetActive(state);
    }

    public void AddBarrierPoint()
    {
        totalBarrierScore++;
        Debug.Log($"[BARRIER] Score: {totalBarrierScore}");
        UpdatePlayerScoreDisplay();
    }

    public void AddBiodiversity(EcosystemZone.EcosystemType type)
    {
        switch (type)
        {
            case EcosystemZone.EcosystemType.Freshwater:
                freshwaterCount++;
                break;
            case EcosystemZone.EcosystemType.Marine:
                marineCount++;
                break;
            case EcosystemZone.EcosystemType.Terrestrial:
                terrestrialCount++;
                break;
        }
        UpdatePlayerScoreDisplay();
    }
    #endregion

    public void SetProtectionMultiplierLevel(int level)
    {
        if (level < 0 || level > 4) return;
        SetProtectionLevel((ProtectionLevel)level);
    }
}
