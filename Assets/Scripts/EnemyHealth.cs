using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("«доровье")]
    public int maxHealth = 100; 
    
    [Header("ќчки")]
    public int scoreValue = 100;

    [Header("Ћут и Ёффекты")]
    public GameObject explosionPrefab;
    public GameObject[] lootPrefabs;
    [Range(0, 100)] public float dropChance = 20f;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        Debug.Log($"{gameObject.name} получил урон. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die(true); 
        }
    }

    public void SelfDestruct()
    {
        Die(false); 
    }

    private void Die(bool givePoints)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        if (givePoints && GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue); 
        }

        if (givePoints && Random.Range(0, 100) <= dropChance && lootPrefabs.Length > 0)
        {
            int index = Random.Range(0, lootPrefabs.Length);
            Instantiate(lootPrefabs[index], transform.position, Quaternion.identity);
        }

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}