using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Характеристики")]
    public float speed = 1.5f;       
    public int damage = 10;          
    public float stopDistance = 0.5f;

    private Transform playerTarget;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        transform.LookAt(playerTarget);

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);
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
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }

        EnemyHealth myHealth = GetComponent<EnemyHealth>();

        if (myHealth != null)
        {
            myHealth.SelfDestruct();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}