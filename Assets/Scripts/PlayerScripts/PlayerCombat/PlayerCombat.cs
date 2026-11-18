using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerCombat : MonoBehaviour
{
    // Reference to movement controller + animator
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    // Scriptable Object
    [Header("Equipped Weapon")]
    public PlayerCombatData currentWeapon;

    [Header("Combo Tracking")]
    public int currentComboStep = 0;
    private float lastAttackTime;
    public LayerMask targetLayer;

    void Start()
    {
        // Connects the references to script
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {

        if (currentComboStep > 0 && Time.time - lastAttackTime > currentWeapon.comboResetTime)
        {
            currentComboStep = 0;
        }
    }

    // connects dodge input value to player input system
    private void OnMeleeAttack(InputValue inputValue)
    {
        Debug.Log("Attacking!");
        // if animation finish and if dodging or not on ground don't do
        if (Time.time - lastAttackTime > currentWeapon.attackDuration && !thirdPersonController.isDodging || !thirdPersonController.isGrounded)
        {
            PerformMeleeAttack();
        }
    }

    private void PerformMeleeAttack()
    {
        // Put your OverlapSphere damage logic and animation triggers here
        Debug.Log("Attacking!");
    }
}





//private void ExecuteComboHit()
//{
//    currentComboStep++;
//    if (currentComboStep > 3) currentComboStep = 1;

//    lastAttackTime = Time.time;

//    if (animator != null)
//    {
//        animator.SetTrigger("Attack" + currentComboStep);
//    }

//    // Calculate damage using the weapon's base damage
//    float damageToDeal = currentWeapon.baseDamage;
//    if (currentComboStep == 3)
//    {
//        damageToDeal = currentWeapon.baseDamage * 2f;
//    }

//    ApplyDamageInSphere(damageToDeal);
//}

//private void ApplyDamageInSphere(float damageAmount)
//{
//    // Use the weapon's specific attack radius
//    Collider[] detected = Physics.OverlapSphere(transform.position, currentWeapon.attackRadius, targetLayer);
//    foreach (Collider hit in detected)
//    {
//        PlayerHealthManager playerHealth = hit.GetComponent<PlayerHealthManager>();
//        if (playerHealth != null)
//        {
//            playerHealth.TakeDamage(damageAmount);
//        }
//    }
//}