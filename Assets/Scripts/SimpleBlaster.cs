using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleBlaster : MonoBehaviour
{
    [Header("Настройки оружия")]
    public Transform muzzlePoint;
    public float range = 50f;     
    public LayerMask hitLayers;   

    [Header("Снаряд")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;

    [Header("Боевые параметры")]
    public int weaponDamage = 25;
    private int defaultDamage;    // Чтобы запомнить норму
    private Coroutine boostCoroutine; // Чтобы не накладывать эффекты друг на друга

    [Header("Визуал")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public Animator gunAnimator;
    public string triggerName = "Fire";

    private LineRenderer laserLine;

    void Start()
    {
        laserLine = muzzlePoint.GetComponent<LineRenderer>();
        // Если не выставить слои в инспекторе, ставим "Все" по умолчанию
        if (hitLayers == 0) hitLayers = ~0;
        defaultDamage = weaponDamage;
    }

    void Update()
    {
        DrawLaser(); 

        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.isGameActive || GameManager.Instance.isPaused) return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Shoot();
        }
    }

    public void ActivateDamageBoost(int multiplier, float duration)
    {
        // Если бонус уже действует, сбрасываем таймер (останавливаем старую корутину)
        if (boostCoroutine != null) StopCoroutine(boostCoroutine);

        boostCoroutine = StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    System.Collections.IEnumerator DamageBoostRoutine(int multiplier, float duration)
    {
        // 1. Усиливаем
        weaponDamage = defaultDamage * multiplier;
        Debug.Log($"DAMAGE BOOST ACTIVATED! New Damage: {weaponDamage}");

        // (Тут можно включить какой-то звук или поменять цвет лазера на время)

        // 2. Ждем
        yield return new WaitForSeconds(duration);

        // 3. Возвращаем как было
        weaponDamage = defaultDamage;
        boostCoroutine = null;
        Debug.Log("Damage Boost Ended.");
    }

    void DrawLaser()
    {
        laserLine.SetPosition(0, muzzlePoint.position);

        RaycastHit hit;
        // Используем hitLayers, чтобы луч видел стены
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range, hitLayers))
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
        if (gunAnimator != null) gunAnimator.SetTrigger(triggerName);
        if (muzzleFlash != null) muzzleFlash.Play();
        if (gunAudio != null) gunAudio.Play();

        RaycastHit hit;
        Vector3 targetPoint;
        EnemyHealth targetEnemy = null;

        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range, hitLayers))
        {
            targetPoint = hit.point;
            targetEnemy = hit.collider.GetComponent<EnemyHealth>();
        }
        else
        {
            targetPoint = muzzlePoint.position + muzzlePoint.forward * range;
        }

        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.identity);
            BulletMover mover = bullet.GetComponent<BulletMover>();

            if (mover != null)
            {
                mover.speed = bulletSpeed;
                // ПЕРЕДАЕМ weaponDamage
                mover.Setup(targetPoint, targetEnemy, weaponDamage);
            }
        }
    }
}