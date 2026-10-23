using UnityEngine;
using static PlayerHealthManager;

/// <summary>
/// DEMO: Physics.OverlapSphere
///
/// Attach this to any GameObject. Every frame, it checks whether any
/// Collider on Target Layer is touching an invisible sphere of Radius,
/// centered on this object.
///
/// Run the scene, select this object in the Hierarchy, and watch the
/// Scene view: the wire sphere turns from green to red the instant
/// something enters it, and anything detected gets a yellow outline.
/// If Feedback Renderer is assigned, its material also changes colour
/// live in the Game view - no Scene view required to see it working.
/// </summary>
public class OverlapSphereDemo : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("How far around this object to check.")]
    public float radius = 3f;

    [Tooltip("Only objects on this layer will be detected.")]
    public LayerMask targetLayer;

    [Header("Live Feedback (optional)")]
    [Tooltip("Drag a Renderer here (e.g. this object's own) to see a colour change in the Game view too.")]
    public Renderer feedbackRenderer;
    public Color noDetectionColor = Color.white;
    public Color detectionColor = Color.red;

    // Stored each frame so OnDrawGizmosSelected can show the same
    // result Update() just calculated, instead of running the query twice.
    private Collider[] detected;
    private bool isDetecting;

    // damage calc
    public float onEntryDamage = 0.1f;
    private float damageInterval = 2f;
    private float timeCheck = 0f;

    private void Update()
    {
        // Ask Unity: what Colliders on targetLayer are touching an
        // invisible sphere of this radius, centered on this object?
        detected = Physics.OverlapSphere(transform.position, radius, targetLayer);
        isDetecting = detected.Length > 0;

        if (feedbackRenderer != null)
        {
            feedbackRenderer.material.color = isDetecting ? detectionColor : noDetectionColor;
        }

        if (isDetecting && Time.time >= timeCheck)
        {
            Attack();

            timeCheck = Time.time + damageInterval;
        }
    }

    private void Attack()
    {
        foreach (Collider hit in detected)
        {
            PlayerHealthManager playerHealth = hit.GetComponent<PlayerHealthManager>();
            playerHealth.TakeDamage(onEntryDamage);
        }
    }

    // Only draws while this object is selected in the Hierarchy -
    // see the lab background deck for why OnDrawGizmosSelected is
    // usually the better default over OnDrawGizmos.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isDetecting ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (detected == null) return;

        Gizmos.color = Color.yellow;
        foreach (Collider hit in detected)
        {  
            Gizmos.DrawWireCube(hit.bounds.center, hit.bounds.size);
        }
    }
}
