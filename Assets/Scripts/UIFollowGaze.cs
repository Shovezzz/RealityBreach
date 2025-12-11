using UnityEngine;

public class UIFollowGaze : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Насколько далеко от игрока висит меню")]
    public float distance = 2.0f;

    [Tooltip("Смещение по высоте (0 = на уровне глаз, отрицательное = ниже)")]
    public float heightOffset = -0.2f;

    [Tooltip("Скорость следования (чем меньше, тем плавнее)")]
    public float smoothSpeed = 5.0f;

    private Transform head;

    void Start()
    {
        if (Camera.main != null)
        {
            head = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (head == null) return;

        Vector3 lookDirection = head.forward;
        lookDirection.y = 0;
        lookDirection.Normalize();

        Vector3 targetPosition = head.position + (lookDirection * distance);
        targetPosition.y = head.position.y + heightOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * smoothSpeed);

        Vector3 directionToHead = transform.position - head.position;
        directionToHead.y = 0;

        if (directionToHead != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToHead);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * smoothSpeed);
        }
    }
}