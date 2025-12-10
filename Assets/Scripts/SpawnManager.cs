using System.Collections;
using UnityEngine;
using Meta.XR.MRUtilityKit; 

public class SpawnManager : MonoBehaviour
{
    [Header("Настройки спауна")]
    public GameObject enemyPrefab;   // Кого спауним
    public float spawnInterval = 2.0f; // Раз в сколько секунд

    private float _timer;

    void Start()
    {
        _timer = spawnInterval;
    }

    void Update()
    {
        // Если MRUK не инициализирован или комната не загружена — ждем
        if (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom() == null) return;
        if (GameManager.Instance != null && !GameManager.Instance.isGameActive) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            SpawnEnemy();
            _timer = spawnInterval;
        }
    }

    public void SpawnEnemy()
    {
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

            // Поворачиваем врага спиной к стене
            newEnemy.transform.rotation = Quaternion.LookRotation(normal);

            Debug.Log("Враг появился на стене!");
        }
    }
}