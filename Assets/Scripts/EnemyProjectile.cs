using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    void Start()
    {
        // Уничтожить через 5 сек, если не попал
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Летим вперед
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Если попали в Игрока (или в камеру, где висит HP)
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Если попали в стену (Environment)
        else if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) // Или проверка по тегу
        {
            Destroy(gameObject);
        }
    }
}