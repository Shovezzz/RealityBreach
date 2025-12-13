using UnityEngine;
using Meta.XR.MRUtilityKit; // Подключаем Meta SDK

public class RoomLayerFixer : MonoBehaviour
{
    [Tooltip("Имя слоя, который нужно присвоить стенам и мебели")]
    public string targetLayerName = "Environment";

    void Start()
    {
        // Подписываемся на событие загрузки комнаты
        if (MRUK.Instance != null)
        {
            MRUK.Instance.RegisterSceneLoadedCallback(OnSceneLoaded);
        }
    }

    // Этот метод сработает автоматически, когда комната загрузится
    void OnSceneLoaded()
    {
        int layerID = LayerMask.NameToLayer(targetLayerName);

        if (layerID == -1)
        {
            Debug.LogError($"Слой {targetLayerName} не найден! Проверь настройки слоев.");
            return;
        }

        // Получаем текущую комнату
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room == null) return;

        // Проходимся по всем объектам (Якорям) в комнате (Стены, Пол, Столы...)
        foreach (var anchor in room.Anchors)
        {
            if (anchor != null && anchor.gameObject != null)
            {
                SetLayerRecursively(anchor.gameObject, layerID);
            }
        }

        // Дополнительно: ищем объект EffectMesh и его детей, на случай если они отдельно
        GameObject effectMesh = GameObject.Find("EffectMesh");
        // (Или EffectMesh(Clone), имя может отличаться, но поиск по якорям выше надежнее)
        if (effectMesh != null)
        {
            SetLayerRecursively(effectMesh, layerID);
        }

        Debug.Log($"Слой {targetLayerName} успешно применен к комнате!");
    }

    // Рекурсивная функция (меняет слой объекту и всем его детям)
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}