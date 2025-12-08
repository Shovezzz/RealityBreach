using UnityEngine;
using TMPro; // Для работы с текстом

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Синглтон (чтобы обращаться отовсюду)

    [Header("UI Ссылки")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hpText;

    private int score = 0;

    void Awake()
    {
        // Делаем этот скрипт доступным для всех
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Вызывается, когда убили врага
    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    // Вызывается, когда игрока ударили
    public void UpdateHP(int currentHP)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP}";

            // Меняем цвет на красный, если мало HP
            if (currentHP <= 30) hpText.color = Color.red;
            else hpText.color = Color.green;
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}