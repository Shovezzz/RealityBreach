using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject explosionPrefab;

    public void TakeDamage()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }
        Die();
    }

    public void SelfDestruct()
    {
        Die();
    }

    private void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}