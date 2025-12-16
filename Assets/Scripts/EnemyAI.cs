using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged } // Типы врагов

    [Header("Тип Врага")]
    public EnemyType combatType = EnemyType.Melee;

    [Header("Общие Характеристики")]
    public float speed = 1.5f;
    public int contactDamage = 10; // Урон при столкновении (для всех)

    [Header("Настройки Стрелка (Только для Ranged)")]
    public GameObject projectilePrefab; // Чем стрелять
    public Transform firePoint;         // Откуда стрелять
    public float shootingRange = 3.0f;  // Дистанция стрельбы
    public float fireRate = 2.0f;       // Пауза между выстрелами
    public AudioClip shootSound; // <--- НОВАЯ ПЕРЕМЕННАЯ (Звук выстрела)

    [Header("Появление")]
    public float spawnMoveTime = 1.5f;
    public AudioClip spawnSound;

    [Header("Навигация")]
    public float obstacleCheckDistance = 1.0f;
    public LayerMask obstacleMask;
    public float bodyRadius = 0.3f;

    private Transform playerTarget;
    private bool isSpawning = true;
    private AudioSource audioSource;
    private float fireTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (spawnSound != null) audioSource.PlayOneShot(spawnSound);

        if (Camera.main != null) playerTarget = Camera.main.transform;
        if (obstacleMask == 0) obstacleMask = ~0;

        StartCoroutine(SpawnRoutine());

        // Чтобы стрелок не выстрелил мгновенно
        fireTimer = fireRate;
    }

    IEnumerator SpawnRoutine()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnMoveTime);
        isSpawning = false;
    }

    void Update()
    {
        // 1. Фаза Спауна (Вылет из стены)
        if (isSpawning)
        {
            // Просто летим вперед сквозь всё заданное время
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        if (playerTarget == null) return;

        // Поворот к игроку
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // --- ЛОГИКА ПОВЕДЕНИЯ ---

        if (combatType == EnemyType.Melee)
        {
            // БЛИЖНИЙ БОЙ (Танк, Скаут)
            // Просто летим к игроку, пока не врежемся
            MoveToPlayer(directionToPlayer, distance, 0.5f); // 0.5 - дистанция атаки телом
        }
        else if (combatType == EnemyType.Ranged)
        {
            // СТРЕЛОК
            // Летим к игроку, но останавливаемся на дистанции выстрела
            if (distance > shootingRange)
            {
                MoveToPlayer(directionToPlayer, distance, shootingRange);
            }
            else
            {
                // Стоим и стреляем
                fireTimer -= Time.deltaTime;
                if (fireTimer <= 0)
                {
                    Shoot();
                    fireTimer = fireRate;
                }
            }
        }

        // Если любой враг подошел вплотную - бьем телом (на всякий случай)
        if (distance <= 0.6f)
        {
            AttackPlayerMelee();
        }
    }

    void MoveToPlayer(Vector3 directionToPlayer, float currentDistance, float stopDist)
    {
        Vector3 moveDirection = directionToPlayer;

        // Обход препятствий
        if (Physics.SphereCast(transform.position, bodyRadius, transform.forward, out RaycastHit hit, obstacleCheckDistance, obstacleMask))
        {
            moveDirection = Vector3.ProjectOnPlane(directionToPlayer, hit.normal).normalized;
            moveDirection += hit.normal * 0.2f;
            moveDirection.Normalize();
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // 1. Создаем пулю
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // 2. Поворачиваем на игрока
            if (playerTarget != null)
            {
                bullet.transform.LookAt(playerTarget.position);
            }

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }

    void AttackPlayerMelee()
    {
        if (playerTarget != null)
        {
            PlayerHealth hp = playerTarget.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(contactDamage);
        }

        EnemyHealth myHealth = GetComponent<EnemyHealth>();
        if (myHealth != null) myHealth.SelfDestruct();
        else Destroy(gameObject);
    }
}