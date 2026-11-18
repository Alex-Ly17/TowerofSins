using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHealthData", menuName = "Scriptable Objects/PlayerHealthData")]

// Store player max and current health
public class PlayerHealthData : ScriptableObject
{
    public float playerMaxHealth = 100;
    public float playerCurrentHealth = 100;
}
