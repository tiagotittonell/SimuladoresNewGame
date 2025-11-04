using UnityEngine;

public class FrameFreezeLogger : MonoBehaviour
{
    float lastTime;

    void Update()
    {
        float now = Time.realtimeSinceStartup;
        float delta = now - lastTime;
        lastTime = now;

        // Si el frame se tarda más de 0.05s (50ms), logueamos
        if (delta > 0.05f)
        {
            Debug.LogWarning($"⚠ Tirón detectado: frame tardó {delta * 1000f:F1} ms en {Time.frameCount}");
        }
    }
}
