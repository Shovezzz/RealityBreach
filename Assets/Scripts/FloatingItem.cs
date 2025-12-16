using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("Вращение")]
    public float rotationSpeed = 50f;
    // НОВОЕ: Ось вращения (X, Y, Z). Ставь 1 там, где надо крутить.
    public Vector3 rotationAxis = new Vector3(0, 1, 0);
    public bool useGlobalAxis = true; // Крутить вокруг Мировой оси (ровно) или Локальной (криво)

    [Header("Покачивание")]
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 2.0f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 1. Вращение
        if (useGlobalAxis)
        {
            // Крутим вокруг мировой оси Y (всегда ровно вверх), даже если куб наклонен
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Крутим вокруг своей оси
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // 2. Покачивание
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}