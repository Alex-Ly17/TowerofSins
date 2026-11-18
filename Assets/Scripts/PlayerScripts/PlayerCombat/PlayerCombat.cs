//using System.Collections;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using Unity.Cinemachine;

//public class PlayerCombat : MonoBehaviour
//{
//    // Reference to movement controller + animator
//    private ThirdPersonController thirdPersonController;
//    private Animator animator;

//    // Scriptable Object
//    [Header("Equipped Weapon")]
//    public WeaponData currentWeapon; 

//    [Header("Combo Tracking")]
//    public int currentComboStep = 0;
//    private float lastAttackTime;
//    public LayerMask targetLayer;

//    void Start()
//    {
//        // Connects the references to script
//        thirdPersonController = GetComponent<ThirdPersonController>();
//        animator = GetComponentInChildren<Animator>();
//    }

//    void Update()
//    {

//        if (currentComboStep > 0 && Time.time - lastAttackTime > currentWeapon.comboResetTime)
//        {
//            currentComboStep = 0;
//        }
//    }

//    // connects dodge input value to player input system
//    private void OnMeleeAttack(InputValue inputValue)
//    {
//        // if dodging or not on ground don't do
//        if (thirdPersonController.isDodging || !thirdPersonController.isgrounded || )
//        {
//            PerformMeleeAttack();
//        }
//    }

//    private void PerformMeleeAttack()
//    {
//        // Put your OverlapSphere damage logic and animation triggers here
//        Debug.Log("Attacking!");
//    }
//}





//void Update()
//{
//    // Safety check: Do not allow attacking if no weapon is equipped
//    if (currentWeapon == null) return;

//    // 1. Reset combo using the weapon's specific reset timer
//    if (currentComboStep > 0 && Time.time - lastAttackTime > currentWeapon.comboResetTime)
//    {
//        currentComboStep = 0;
//    }

//    // 2. Read attack input using the weapon's specific swing speed
//    if (Input.GetMouseButtonDown(0))
//    {
//        if (!movementController.isDodging && Time.time - lastAttackTime > currentWeapon.attackDuration)
//        {
//            ExecuteComboHit();
//        }
//    }
//}

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