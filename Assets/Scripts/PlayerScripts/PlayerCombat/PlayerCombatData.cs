using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCombatData", menuName = "Scriptable Objects/PlayerCombatData")]
public class PlayerCombatData : ScriptableObject
{
    [Header("Combat Stats")]
    public float baseDamage1 = 10f;
    public float attackRadius = 1.5f;
    public float attackDowntime = 1f; 
}


