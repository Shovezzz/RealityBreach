using UnityEngine;

[RequireComponent(typeof(LineRenderer))] // Автоматически добавит компонент
public class SimpleBlaster : MonoBehaviour
{
    public Transform muzzlePoint;
    public float range = 100f;

    private LineRenderer laserLine;

    void Start()
    {
        laserLine = GetComponent<LineRenderer>();

        // Настройка лазера через код (чтобы не возиться в инспекторе)
        laserLine.startWidth = 0.01f;
        laserLine.endWidth = 0.01f;
        laserLine.material = new Material(Shader.Find("Sprites/Default")); // Простой белый материал
        laserLine.startColor = Color.red;
        laserLine.endColor = Color.red;
    }

    void Update()
    {
        // Рисуем лазер всегда
        DrawLaser();

        // Стрельба (Курок или Пробел)
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void DrawLaser()
    {
        laserLine.SetPosition(0, muzzlePoint.position); // Начало линии

        // Конец линии (либо точка удара, либо макс. дистанция)
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range))
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, muzzlePoint.position + muzzlePoint.forward * range);
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range))
        {
            // Проверка на врага
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
        }
    }
}