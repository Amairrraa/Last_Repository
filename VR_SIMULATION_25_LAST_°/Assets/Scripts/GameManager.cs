using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Over Panel Settings")]
    public TextMeshProUGUI finalScoreText;

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

    private float timer;
    private bool gameActive = false;
    private bool ropesShown = false;

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

        Debug.Log($"[SCORE] (Barrier: {totalBarrierScore} + " +
                 $"(Bio: {TotalBiodiversityScore} × DF: {distributionFactor:F2})) × " +
                 $"PM: {_currentMultiplier:F3} = {finalScore:F2}\n" +
                 $"Breakdown:\n" +
                 $"- Barriers: {totalBarrierScore}\n" +
                 $"- Biodiversity: {TotalBiodiversityScore} × {distributionFactor:F2} = {biodiversityComponent:F2}\n" +
                 $"- Subtotal Before PM: {totalBarrierScore + biodiversityComponent:F2}\n" +
                 $"- After Protection Multiplier (×{_currentMultiplier:F3}): {finalScore:F2}");

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
            finalScoreText.text = finalScore.ToString("F0");

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
    }

    public void AddBiodiversity(EcosystemZone.EcosystemType type)
    {
        switch (type)
        {
            case EcosystemZone.EcosystemType.Freshwater:
                freshwaterCount++;
                Debug.Log("[BIODIVERSITY] +1 Freshwater 💧");
                break;
            case EcosystemZone.EcosystemType.Marine:
                marineCount++;
                Debug.Log("[BIODIVERSITY] +1 Marine 🌊");
                break;
            case EcosystemZone.EcosystemType.Terrestrial:
                terrestrialCount++;
                Debug.Log("[BIODIVERSITY] +1 Terrestrial 🌳");
                break;
        }
        Debug.Log($"[BIODIVERSITY] Total: {TotalBiodiversityScore} 🌍");
    }
    #endregion
    public void SetProtectionMultiplierLevel(int level)
    {
        if (level < 0 || level > 4) return;
        SetProtectionLevel((ProtectionLevel)level);
    }
}