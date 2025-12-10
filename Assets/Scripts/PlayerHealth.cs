using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public AudioSource hitAudio;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameManager.Instance.UpdateHP(currentHealth);
        if (hitAudio != null) hitAudio.Play();
        Debug.Log($"Игрок получил урон! HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        // Тут потом добавим остановку игры и экран смерти
        Time.timeScale = 0; // Пауза игры
    }
}