using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] // Скрипт работает прямо в редакторе, не надо запускать игру
public class ForceAlwaysVisible : MonoBehaviour
{
    void Start()
    {
        ApplyZTest();
    }

    void OnValidate()
    {
        ApplyZTest();
    }

    void ApplyZTest()
    {
        // Ищем картинку (Image) на этом объекте
        Image img = GetComponent<Image>();
        if (img != null && img.material != null)
        {
            // Создаем копию материала, чтобы не испортить оригинал на диске (если нужно)
            // Но для удобства в редакторе мы меняем текущий.

            // Магическая команда: ZTest = 8 (Always)
            img.material.SetInt("unity_GUIZTestMode", 8);
        }
    }
}