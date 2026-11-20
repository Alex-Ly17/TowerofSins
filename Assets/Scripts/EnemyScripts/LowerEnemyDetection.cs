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
public class LowerEnemyDetection : MonoBehaviour
{
    private Animator animator;

    [Header("Detection Settings")]
    [Tooltip("How far around this object to check.")]
    public float radius = 3f;

    [Tooltip("Only objects on this layer will be detected.")]
    public LayerMask targetLayer;

    // Stored each frame so OnDrawGizmosSelected can show the same
    // result Update() just calculated, instead of running the query twice.
    private Collider[] detected;
    private bool isDetecting;

    // damage calc
    public float onEntryDamage = 10;
    private float damageInterval = 2f;
    private float timeCheck = 0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Ask Unity: what Colliders on targetLayer are touching an
        // invisible sphere of this radius, centered on this object?
        detected = Physics.OverlapSphere(transform.position, radius, targetLayer);
        isDetecting = detected.Length > 0;

        if (isDetecting && Time.time >= timeCheck)
        {
            StartAttack();
            timeCheck = Time.time + damageInterval;
        }
    }

    private void StartAttack()
    {
        foreach (Collider hit in detected)
        {
            animator.SetTrigger("attack");
        }
    }

    private void DealAttackDamage()
    {
        // Re-check the sphere right now so the player can dodge out of range mid-punch
        Collider[] hitsAtImpact = Physics.OverlapSphere(transform.position, radius, targetLayer);

        foreach (Collider hit in hitsAtImpact)
        {
            PlayerHealthManager playerHealth = hit.GetComponentInParent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(onEntryDamage);
                break; 
            }
        }
    }
}
