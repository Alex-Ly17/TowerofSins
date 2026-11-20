using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerCombat : MonoBehaviour
{
    // Reference to movement controller + animator
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    //
    public bool canAttack = true;
 
    // Scriptable Object
    [Header("Equipped Weapon")]
    public PlayerCombatData currentWeapon;

    [Header("Combo Tracking")]
    public LayerMask targetLayer;

    void Start()
    {
        // Connect references
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Connects melee input value to player input system
    private void OnMeleeAttack(InputValue inputValue)
    {
        MeleeAttack();
    }

    // Do melee attack
    private void MeleeAttack()
    {
        // can't attack if player in air or in action already
        if (!thirdPersonController.isGrounded || thirdPersonController.inAction)
        {
            return;
        }

        // tells the game player is currently attacking
        thirdPersonController.inAction = true;
        canAttack = false;

        // next attack timer
        StartCoroutine(AttackDowntimeCoroutine(skipLaunchTime: false));

        // animation + deal damage
        float damageToDeal;
        animator.SetTrigger("comboStep1");
        damageToDeal = currentWeapon.baseDamage1;
        ApplyDamageInSphere(damageToDeal);
    }

    // attack cooldown
    private IEnumerator AttackDowntimeCoroutine(bool skipLaunchTime)
    {
        if (!skipLaunchTime)
        {
            // wait for players 
            yield return new WaitForSeconds(0.25f);
        }

        // wait for attack down time before player can melee again
        yield return new WaitForSeconds(currentWeapon.attackDowntime);
        canAttack = true;
        thirdPersonController.inAction = false;
    }

    // apply damage in radius
    private void ApplyDamageInSphere(float damageAmount)
    {
        Collider[] detected = Physics.OverlapSphere(transform.position, currentWeapon.attackRadius, targetLayer);

        foreach (Collider hit in detected)
        {
            EnemyHealthManager enemyHealth = hit.GetComponentInParent<EnemyHealthManager>();
            if (enemyHealth != null)
            {
                enemyHealth.EnemyTakeDamage(damageAmount);
            }
        }
    }
}

