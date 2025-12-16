using UnityEngine;

public class TimeAbility : MonoBehaviour
{
    [Header("Настройки")]
    public float slowFactor = 0.3f;
    public float abilityDuration = 3.0f;
    public float cooldown = 10.0f;

    [Header("Звук")]
    public AudioSource abilityAudio;
    public AudioClip slowSound;    
    public AudioClip readySound;   

    private float timer;
    private bool isActive = false;
    private float cooldownTimer = 0;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameActive) return;

        // --- ЛОГИКА ПЕРЕЗАРЯДКИ ---
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;

            // Если таймер только что дошел до нуля
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                // ИГРАЕМ ЗВУК "ГОТОВО" ЗДЕСЬ
                if (abilityAudio != null && readySound != null)
                {
                    abilityAudio.PlayOneShot(readySound);
                }
                Debug.Log("Ability Ready!");
            }
        }
        // ---------------------------

        // Включение (Левый курок)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && cooldownTimer <= 0 && !isActive)
        {
            StartSlowMo();
        }

        // Таймер действия эффекта
        if (isActive)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0)
            {
                StopSlowMo();
            }
        }
    }

    void StartSlowMo()
    {
        isActive = true;
        timer = abilityDuration;

        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (abilityAudio && slowSound) abilityAudio.PlayOneShot(slowSound);
    }

    void StopSlowMo()
    {
        isActive = false;
        cooldownTimer = cooldown; // Ставим таймер (например, 10 сек)

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        // ОТСЮДА ЗВУК УБРАЛИ. Теперь он заиграет только когда cooldownTimer станет 0.
    }
}