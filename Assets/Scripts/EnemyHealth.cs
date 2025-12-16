using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Здоровье")]
    public int maxHealth = 100; // Настраивается в инспекторе

    [Header("Лут и Эффекты")]
    public GameObject explosionPrefab;
    public GameObject[] lootPrefabs;
    [Range(0, 100)] public float dropChance = 20f;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // ТЕПЕРЬ МЕТОД ПРИНИМАЕТ УРОН
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Можно добавить эффект мигания или звук попадания здесь
        Debug.Log($"{gameObject.name} получил урон. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die(true); // true = дать очки
        }
    }

    // Самоуничтожение (когда врезался в игрока)
    public void SelfDestruct()
    {
        Die(false); // false = без очков
    }

    private void Die(bool givePoints)
    {
        // 1. Эффект взрыва
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // 2. Очки (только если убил игрок)
        if (givePoints && GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100); // Можно сделать разным для разных врагов
        }

        // 3. Лут (только если убил игрок)
        if (givePoints && Random.Range(0, 100) <= dropChance && lootPrefabs.Length > 0)
        {
            int index = Random.Range(0, lootPrefabs.Length);
            Instantiate(lootPrefabs[index], transform.position, Quaternion.identity);
        }

        // 4. Уведомление спаунера
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}