using UnityEngine;

public class BulletMover : MonoBehaviour
{
    public float speed = 20f; 
    public GameObject hitEffectPrefab; 

    private Vector3 targetPosition;
    private EnemyHealth targetEnemy; 
    private bool isMoving = false;

    public void Setup(Vector3 target, EnemyHealth enemy)
    {
        targetPosition = target;
        targetEnemy = enemy;

        transform.LookAt(targetPosition);
        isMoving = true;

        // уничтожить через 3 сек, если что-то пошло не так
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (!isMoving) return;

        // MoveTowards гарантирует, что мы точно придем в точку, не перелетев её
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Проверяем, долетели ли мы (дистанция почти ноль)
        if (Vector3.Distance(transform.position, targetPosition) < 0.005f)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (targetEnemy != null)
        {
            if (targetEnemy.gameObject != null)
            {
                targetEnemy.TakeDamage(); 
            }
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}