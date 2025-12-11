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
                mover.Setup(targetPoint, targetEnemy);
            }
        }
    }
}