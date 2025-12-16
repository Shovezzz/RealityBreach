using System.Collections;
using System.Collections.Generic; // Нужно для списков
using UnityEngine;
using Meta.XR.MRUtilityKit;

[System.Serializable]
public struct EnemyWaveConfig
{
    public GameObject prefab;
    public int unlockWave; // С какой волны начинает появляться этот враг
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Враги и Прогрессия")]
    public List<EnemyWaveConfig> enemyConfigs; // Сюда закинем наши префабы

    [Header("Настройки")]
    public float timeBetweenWaves = 3.0f;
    public int baseEnemies = 3;
    
    [Header("Эффекты")]
    public GameObject portalPrefab;

    // ... (остальные переменные: currentWave, enemiesToSpawn и т.д. остаются) ...
    [SerializeField] private int currentWave = 1;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int enemiesAlive;
    private bool isWaveActive = false;
    private float spawnTimer = 0f;

    void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    // ... Update() ОСТАВЛЯЕМ БЕЗ ИЗМЕНЕНИЙ ...
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

    // ... StartFirstWave и StartWaveRoutine ОСТАВЛЯЕМ БЕЗ ИЗМЕНЕНИЙ ...
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

    // --- ГЛАВНОЕ ИЗМЕНЕНИЕ ЗДЕСЬ ---
    void SpawnEnemy()
    {
        enemiesToSpawn--;

        if (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom() == null) return;

        // Выбираем врага
        GameObject selectedPrefab = GetRandomEnemyForCurrentWave();

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        LabelFilter filter = new LabelFilter(MRUKAnchor.SceneLabels.WALL_FACE);

        // Ищем точку на стене
        bool positionFound = room.GenerateRandomPositionOnSurface(
            (MRUK.SurfaceType)~0,
            0.1f,
            filter,
            out Vector3 surfacePos, // Это точка ПРЯМО НА СТЕНЕ
            out Vector3 normal
        );

        if (positionFound)
        {
            // --- 1. СПАУНИМ ПОРТАЛ ---
            if (portalPrefab != null)
            {
                // Сдвигаем портал на 1 см от стены, чтобы он не мерцал (Z-fighting)
                Vector3 portalPos = surfacePos + (normal * 0.05f);
                GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity);

                // Поворачиваем портал так, чтобы он лежал на стене
                portal.transform.rotation = Quaternion.LookRotation(normal);
            }
            // -------------------------

            // --- 2. СПАУНИМ ВРАГА (Глубоко в стене) ---
            // Сдвигаем точку спауна врага на 0.5 метра ВГЛУБЬ стены (против нормали)
            Vector3 enemySpawnPos = surfacePos - (normal * 0.5f);

            GameObject newEnemy = Instantiate(selectedPrefab, enemySpawnPos, Quaternion.identity);

            // Поворачиваем врага лицом "из стены"
            newEnemy.transform.rotation = Quaternion.LookRotation(normal);
        }
    }

    // Логика выбора врага
    GameObject GetRandomEnemyForCurrentWave()
    {
        // Собираем список всех, кто доступен на этой волне
        List<GameObject> availableEnemies = new List<GameObject>();

        foreach (var config in enemyConfigs)
        {
            if (currentWave >= config.unlockWave)
            {
                availableEnemies.Add(config.prefab);
            }
        }

        // Если список пуст (ошибка настройки), берем первого
        if (availableEnemies.Count == 0) return enemyConfigs[0].prefab;

        // Возвращаем случайного
        return availableEnemies[Random.Range(0, availableEnemies.Count)];
    }

    // ... OnEnemyKilled и WaveCompleted ОСТАВЛЯЕМ БЕЗ ИЗМЕНЕНИЙ ...
    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && enemiesToSpawn <= 0) WaveCompleted();
    }

    void WaveCompleted()
    {
        currentWave++;
        StartCoroutine(StartWaveRoutine());
    }
}