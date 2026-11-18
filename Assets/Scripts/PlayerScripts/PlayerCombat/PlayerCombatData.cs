using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCombatData", menuName = "Scriptable Objects/PlayerCombatData")]
public class PlayerCombatData : ScriptableObject
{
    [Header("Combat Stats")]
    public float baseDamage = 10f;
    public float attackRadius = 1.5f;

    [Header("Combo Timers")]
    public float attackDuration = 0.5f; // How fast this melee attack swings
    public float comboResetTime = 1.2f; // How long before the combo drops
}


