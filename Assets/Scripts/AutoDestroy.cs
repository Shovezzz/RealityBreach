using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("Сколько секунд живет объект перед удалением")]
    public float lifetime = 3.0f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}