using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using Meta.XR.MRUtilityKit;

[System.Serializable]
public struct EnemyWaveConfig
{
    public GameObject prefab;
    public int unlockWave; 
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Враги и Прогрессия")]
    public List<EnemyWaveConfig> enemyConfigs; 

    [Header("Настройки")]
    public float timeBetweenWaves = 3.0f;
    public int baseEnemies = 3;
    
    [Header("Эффекты")]
    public GameObject portalPrefab;

    [SerializeField] private int currentWave = 1;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int enemiesAlive;
    private bool isWaveActive = false;
    private float spawnTimer = 0f;

    void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameActive || GameManager.Instance.isPaused) return;
        if (!isWaveActive) return;

        if (enemiesToSpawn > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy();
                spawnTimer = Mathf.Max(0.5f, 2.0f - (currentWave * 0.1f));
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
        if (GameManager.Instance != null) GameManager.Instance.UpdateWave(currentWave);
        yield return new WaitForSeconds(timeBetweenWaves);

        int totalEnemies = baseEnemies + ((currentWave - 1) * 2);
        enemiesToSpawn = totalEnemies;
        enemiesAlive = totalEnemies;

        isWaveActive = true;
    }

    void SpawnEnemy()
    {
        enemiesToSpawn--;

        if (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom() == null) return;

        GameObject selectedPrefab = GetRandomEnemyForCurrentWave();

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        LabelFilter filter = new LabelFilter(MRUKAnchor.SceneLabels.WALL_FACE);

        bool positionFound = room.GenerateRandomPositionOnSurface(
            (MRUK.SurfaceType)~0,
            0.4f,
            filter,
            out Vector3 surfacePos, 
            out Vector3 normal
        );

        if (positionFound)
        {
            if (portalPrefab != null)
            {
                Vector3 portalPos = surfacePos + (normal * 0.05f);
                GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity);

                portal.transform.rotation = Quaternion.LookRotation(normal);
            }

            Vector3 enemySpawnPos = surfacePos - (normal * 0.5f);

            GameObject newEnemy = Instantiate(selectedPrefab, enemySpawnPos, Quaternion.identity);

            newEnemy.transform.rotation = Quaternion.LookRotation(normal);
        }
    }

    GameObject GetRandomEnemyForCurrentWave()
    {
        List<GameObject> availableEnemies = new List<GameObject>();

        foreach (var config in enemyConfigs)
        {
            if (currentWave >= config.unlockWave)
            {
                availableEnemies.Add(config.prefab);
            }
        }

        if (availableEnemies.Count == 0) return enemyConfigs[0].prefab;

        return availableEnemies[Random.Range(0, availableEnemies.Count)];
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && enemiesToSpawn <= 0) WaveCompleted();
    }

    public int GetCurrentWave() { return currentWave; }

    void WaveCompleted()
    {
        currentWave++;
        StartCoroutine(StartWaveRoutine());
    }
}