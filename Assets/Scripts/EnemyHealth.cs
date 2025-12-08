using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public void TakeDamage()
    {

        Debug.Log("Враг уничтожен!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }
        Destroy(gameObject);

        // Тут позже можно добавить эффекты взрыва
    }
}