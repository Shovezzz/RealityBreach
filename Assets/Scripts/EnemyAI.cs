using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Характеристики")]
    public float speed = 1.5f;       // Скорость полета
    public int damage = 10;          // Урон
    public float stopDistance = 0.5f;// Дистанция атаки

    private Transform playerTarget;

    void Start()
    {
        // Ищем игрока по тегу (голову)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 1. Поворачиваемся к игроку
        transform.LookAt(playerTarget);

        // 2. Считаем дистанцию
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // 3. Если далеко — летим к игроку
        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);
        }
        else
        {
            // 4. Если долетели — атакуем (самоуничтожение или удар)
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        // Пробуем найти здоровье у цели
        PlayerHealth hp = playerTarget.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        // Для простоты: дрон взрывается при касании
        Destroy(gameObject);
    }
}