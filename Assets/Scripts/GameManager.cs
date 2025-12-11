using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Ссылки")]
    public GameObject mainMenuCanvas;
    public GameObject hudCanvas;

    // ПАНЕЛИ
    public GameObject mainPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel; 

    // ТЕКСТЫ
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI finalScoreText;

    // ССЫЛКИ ДЛЯ РЕКОРДА ---
    public TextMeshProUGUI menuBestScoreText; 
    public TextMeshProUGUI gameOverBestScoreText; 

    [Header("Состояние")]
    public bool isGameActive = false;
    public bool isPaused = false;

    private int score = 0;
    private int highScore = 0; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        ShowMainMenu();
    }

    void Update()
    {
        if (isGameActive)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    public void ShowMainMenu()
    {
        isGameActive = false;
        isPaused = false;
        Time.timeScale = 0; 

        mainMenuCanvas.SetActive(true);
        mainPanel.SetActive(true);
        pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 

        hudCanvas.SetActive(false);

        if (menuBestScoreText != null)
        {
            menuBestScoreText.text = $"Best Score: {highScore}";
        }

        CleanupScene();
    }

    public void StartGame()
    {
        score = 0;
        UpdateUI();

        var playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null) playerHealth.ResetHealth();

        if (SpawnManager.Instance != null) SpawnManager.Instance.StartFirstWave();

        isGameActive = true;
        isPaused = false;
        Time.timeScale = 1;

        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
    }

    public void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log("Новый рекорд сохранен!");
        }

        hudCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);

        mainPanel.SetActive(false);
        pausePanel.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null) finalScoreText.text = $"Score: {score}";

            if (gameOverBestScoreText != null)
            {
                gameOverBestScoreText.text = $"Best: {highScore}";
            }
        }

        CleanupScene();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        mainMenuCanvas.SetActive(true);
        mainPanel.SetActive(false);
        pausePanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        hudCanvas.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
    }

    public void QuitGame() { Application.Quit(); }
    public void GoToMainMenu() { ShowMainMenu(); }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void UpdateHP(int currentHP)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP}";
            hpText.color = currentHP <= 30 ? Color.red : Color.green;
        }
    }

    public void UpdateWave(int wave)
    {
        if (waveText != null) waveText.text = $"Wave: {wave}";
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }
    void CleanupScene()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies) Destroy(e);

        var bullets = GameObject.FindObjectsOfType<BulletMover>();
        foreach (var b in bullets) Destroy(b.gameObject);
    }
}