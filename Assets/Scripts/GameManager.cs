using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject mainMenuCanvas; 
    public GameObject hudCanvas;      
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hpText;

    [Header("Состояние")]
    public bool isGameActive = false; 

    private int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowMenu(); 
    }

    public void ShowMenu()
    {
        isGameActive = false;
        mainMenuCanvas.SetActive(true);
        hudCanvas.SetActive(false);

        var enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
        foreach (var e in enemies) Destroy(e);
    }

    public void StartGame()
    {
        score = 0;
        UpdateUI();

        isGameActive = true;
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Выход из игры");
    }

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

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }
}