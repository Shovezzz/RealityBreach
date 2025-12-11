using UnityEngine;

[RequireComponent(typeof(AudioSource))] 
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;

    [Header("Звуки")]
    public AudioClip hitSound; 

    private int currentHealth;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHP(currentHealth);
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (GameManager.Instance != null) GameManager.Instance.UpdateHP(currentHealth);
    }

    void GameOver()
    {
        if (GameManager.Instance != null) GameManager.Instance.GameOver();
    }
}