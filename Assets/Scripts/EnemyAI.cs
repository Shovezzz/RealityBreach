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

    private Transform playerTarget;
    private bool isSpawning = true;    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        if (Camera.main != null)
        {
            playerTarget = Camera.main.transform;
        }

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
        if (isSpawning)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return; 
        }

        if (playerTarget == null) return;

        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

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
            if (hp != null) hp.TakeDamage(damage);
        }

        EnemyHealth myHealth = GetComponent<EnemyHealth>();
        if (myHealth != null) myHealth.SelfDestruct();
        else Destroy(gameObject);
    }
}