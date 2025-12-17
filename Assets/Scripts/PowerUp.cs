using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type { Heal, DamageBoost }
    public Type powerUpType;

    [Header("Параметры")]
    public int value = 20; 

    [Header("Только для Damage Boost")]
    public int damageMultiplier = 2; 
    public float boostDuration = 10.0f; 

    [Header("Звук")]
    public AudioClip pickupSound; 
    [Range(0f, 1f)] public float volume = 1.0f; 

    void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        ApplyBonus();
    }

    void ApplyBonus()
    {
        if (pickupSound != null)
        {
            GameObject playerHead = GameObject.FindGameObjectWithTag("MainCamera");

            if (playerHead != null)
            {
                AudioSource headSource = playerHead.GetComponent<AudioSource>();
                if (headSource != null)
                {
                    headSource.PlayOneShot(pickupSound, volume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(pickupSound, playerHead.transform.position, volume);
                }
            }
        }
        if (GameManager.Instance == null) return;

        if (powerUpType == Type.Heal)
        {
            var player = FindObjectOfType<PlayerHealth>();
            if (player != null) player.Heal(value);
        }
        else if (powerUpType == Type.DamageBoost)
        {
            var blaster = FindObjectOfType<SimpleBlaster>();
            if (blaster != null)
            {
                blaster.ActivateDamageBoost(damageMultiplier, boostDuration);
            }
        }
    }
}