using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    [Header("Характеристики")]
    public float speed = 1.5f;
    public int damage = 10;
    public float stopDistance = 0.5f;

    [Header("Появление")]
    public float spawnMoveTime = 1.5f;
    public AudioClip spawnSound;

    [Header("Навигация")]
    public float obstacleCheckDistance = 1.0f; // Как далеко видеть препятствия
    public LayerMask obstacleMask; // Что считать препятствием (Стены, Мебель)
    public float bodyRadius = 0.3f;

    private Transform playerTarget;
    private bool isSpawning = true;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (spawnSound != null) audioSource.PlayOneShot(spawnSound);

        if (Camera.main != null) playerTarget = Camera.main.transform;

        // Если маска не настроена, ставим "Все слои" по умолчанию
        if (obstacleMask == 0) obstacleMask = ~0;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnMoveTime);
        isSpawning = false;
    }

    void Update()
    {
        // 1. Фаза Спауна (Тут можно оставить пролет сквозь стену, это красиво)
        if (isSpawning)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        if (playerTarget == null) return;

        // Поворот к игроку
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > stopDistance)
        {
            Vector3 moveDirection = directionToPlayer;

            // --- НОВОЕ: Скольжение вдоль препятствий ---
            // Пускаем луч (или шар) вперед
            if (Physics.SphereCast(transform.position, bodyRadius, transform.forward, out RaycastHit hit, obstacleCheckDistance, obstacleMask))
            {
                // Вместо того чтобы лететь ВВЕРХ, мы летим ВДОЛЬ поверхности.
                // ProjectOnPlane берет наш вектор желания (к игроку) и проецирует его на плоскость стены.
                // Получается движение параллельно стене.
                moveDirection = Vector3.ProjectOnPlane(directionToPlayer, hit.normal).normalized;

                // Небольшой хак: чуть-чуть отталкиваемся от стены, чтобы не тереться текстурами
                moveDirection += hit.normal * 0.2f;
                moveDirection.Normalize();
            }
            // ------------------------------------------

            transform.position += moveDirection * speed * Time.deltaTime;
        }
        else
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (playerTarget != null)
        {
            PlayerHealth hp = playerTarget.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(damage);
        }

        EnemyHealth myHealth = GetComponent<EnemyHealth>();
        if (myHealth != null) myHealth.SelfDestruct();
        else Destroy(gameObject);
    }
}