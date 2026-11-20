using UnityEngine;
using System.Collections;

/// <summary>
/// Lab 7 - Goal 3.
///
/// Smoothly animates a Shader Graph's exposed _FlashAmount property from
/// 1 (fully flashed) back to 0 (normal) over flashDuration, using a
/// coroutine so the change happens gradually across several frames
/// instead of snapping instantly.
///
/// Setup:
///   1. Build the unlit Hit Flash (or Dissolve) Shader Graph from the lab,
///      with a Float property named _FlashAmount, Exposed checked.
///   2. Assign that shader's Material to this object's Renderer.
///   3. Drag this object's own Renderer into the Target Renderer field.
///   4. Call OnHit() from wherever your interaction event fires - e.g.
///      MeleeAttackDemo's foreach loop, calling hit.GetComponent
///      <FlashOnHit>()?.OnHit() instead of (or alongside) DemoTarget.Hit().
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    [Header("Shader Link")]
    [Tooltip("The Renderer whose material has the exposed _FlashAmount property.")]
    public Renderer targetRenderer;

    [Tooltip("The exact name of the exposed Float property in the Shader Graph.")]
    public string propertyName = "_FlashAmount";

    [Header("Timing")]
    [Tooltip("How long the flash takes to fade back to 0.")]
    public float flashDuration = 0.2f;

    private readonly List<Material> flashMaterials = new List<Material>();
    private Coroutine activeFlash;

    void Awake()
    {
        if (targetRenderer == null) return;

        // Check ALL material slots (Element 0, Element 1, etc.) instead of only Element 0
        foreach (Material mat in targetRenderer.materials)
        {
            if (mat != null && mat.HasProperty(propertyName))
            {
                flashMaterials.Add(mat);
            }
        }
    }

    /// <summary>
    /// Call this when the object is hit. Safe to call repeatedly. A new
    /// hit restarts the flash from full strength rather than stacking.
    /// </summary>
    public void OnHit()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning($"{name}: No Target Renderer assigned on FlashOnHit.");
            return;
        }

        if (activeFlash != null)
        {
            StopCoroutine(activeFlash);
        }
        activeFlash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float amount = Mathf.Clamp01(1f - (t / flashDuration));
            SetFlashAmount(amount);
            yield return null;
        }

        SetFlashAmount(0f);
        activeFlash = null;
    }

    private void SetFlashAmount(float value)
    {
        for (int i = 0; i < flashMaterials.Count; i++)
        {
            flashMaterials[i].SetFloat(propertyName, value);
        }
    }
}
