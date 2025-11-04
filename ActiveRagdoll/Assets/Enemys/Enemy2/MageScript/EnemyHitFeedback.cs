using UnityEngine;
using System.Collections;

public class EnemyHitFeedback : MonoBehaviour
{
    [Header("Referencias")]
    public Renderer[] renderers; // todos los renderers del enemigo
    public Transform modelTransform; // el modelo que se sacude (no el root del collider)

    [Header("Configuración visual")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.08f;

    [Header("Shake")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    private Color[] originalColors;
    private Material[] materials;
    private Vector3 originalPos;
    private bool isFlashing;

    void Start()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        // Guardar materiales y colores originales
        materials = new Material[renderers.Length];
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            if (materials[i].HasProperty("_Color"))
                originalColors[i] = materials[i].color;
        }

        if (!modelTransform)
            modelTransform = transform;
    }

    /// <summary>
    /// Llamar cuando el enemigo recibe daño.
    /// </summary>
    public void PlayHitFeedback()
    {
        StartCoroutine(HitFlash());
        StartCoroutine(HitShake());
    }

    IEnumerator HitFlash()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        // Tornar blanco
        foreach (var mat in materials)
        {
            if (mat.HasProperty("_Color"))
                mat.color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        // Restaurar color original
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_Color"))
                materials[i].color = originalColors[i];
        }

        isFlashing = false;
    }

    IEnumerator HitShake()
    {
        if (!modelTransform) yield break;

        originalPos = modelTransform.localPosition;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            float strength = Mathf.Lerp(shakeIntensity, 0, timer / shakeDuration);
            modelTransform.localPosition = originalPos + Random.insideUnitSphere * strength;
            timer += Time.deltaTime;
            yield return null;
        }

        modelTransform.localPosition = originalPos;
    }
}
