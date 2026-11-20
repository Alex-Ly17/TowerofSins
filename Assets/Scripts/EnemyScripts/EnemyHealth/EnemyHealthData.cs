using UnityEngine;

[CreateAssetMenu(fileName = "EnemyHealthData", menuName = "Scriptable Objects/EnemyHealthData")]

// Store enemy max and current health
public class EnemyHealthData : ScriptableObject
{
    public float enemyMaxHealth = 100;
    public float enemyCurrentHealth = 100;
}
