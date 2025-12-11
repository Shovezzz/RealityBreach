using System.Collections;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance; 

    [Header("Настройки")]
    public GameObject enemyPrefab;
    public float timeBetweenWaves = 3.0f; 

    [Header("Баланс")]
    public int baseEnemies = 5; 
    public float baseInterval = 2.0f; 

    private int currentWave = 1;
    private int enemiesToSpawn; 
    private int enemiesAlive;   
    private bool isWaveActive = false;
    private float spawnTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameActive || GameManager.Instance.isPaused) return;

        if (!isWaveActive) return;

        // Логика спауна
        if (enemiesToSpawn > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy();
                float currentInterval = Mathf.Max(0.5f, baseInterval - (currentWave * 0.1f));
                spawnTimer = currentInterval;
            }
        }
    }

    public void StartFirstWave()
    {
        currentWave = 1;
        StartCoroutine(StartWaveRoutine());
    }

    IEnumerator StartWaveRoutine()
    {
        isWaveActive = false;

        GameManager.Instance.UpdateWave(currentWave);
        Debug.Log($"--- Волная {currentWave} начинается через {timeBetweenWaves} сек ---");

        yield return new WaitForSeconds(timeBetweenWaves);

        enemiesToSpawn = baseEnemies + ((currentWave - 1) * 2);
        enemiesAlive = enemiesToSpawn;

        isWaveActive = true;
    }

    void SpawnEnemy()
    {
        if (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom() == null) return;

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        LabelFilter filter = new LabelFilter(MRUKAnchor.SceneLabels.WALL_FACE);

        bool positionFound = room.GenerateRandomPositionOnSurface(
            MRUK.SurfaceType.VERTICAL,
            0.1f,
            filter,
            out Vector3 pos,
            out Vector3 normal
        );

        if (positionFound)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            newEnemy.transform.rotation = Quaternion.LookRotation(normal);

            enemiesToSpawn--;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && enemiesToSpawn <= 0)
        {
            WaveCompleted();
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Волна пройдена!");
        currentWave++;
        StartCoroutine(StartWaveRoutine()); 
    }
}