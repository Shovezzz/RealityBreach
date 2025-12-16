using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type { Heal, DamageBoost }
    public Type powerUpType;

    [Header("Параметры")]
    public int value = 20; // Для аптечки (сколько лечить)

    [Header("Только для Damage Boost")]
    public int damageMultiplier = 2; // Во сколько раз усилить
    public float boostDuration = 10.0f; // На сколько секунд

    [Header("Звук")]
    public AudioClip pickupSound; // Сюда закинешь звук (звон, хил, подбор)
    [Range(0f, 1f)] public float volume = 1.0f; // Громкость

    void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        ApplyBonus();
    }

    void ApplyBonus()
    {
        // ... звук ...

        if (GameManager.Instance == null) return;

        // ЛЕЧЕНИЕ
        if (powerUpType == Type.Heal)
        {
            var player = FindObjectOfType<PlayerHealth>();
            if (player != null) player.Heal(value);
        }
        // УСИЛЕНИЕ УРОНА (НОВОЕ)
        else if (powerUpType == Type.DamageBoost)
        {
            // Ищем пистолет на сцене
            var blaster = FindObjectOfType<SimpleBlaster>();
            if (blaster != null)
            {
                blaster.ActivateDamageBoost(damageMultiplier, boostDuration);
            }
        }
    }
}