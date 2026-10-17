using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Scriptable Object Referece")]
    public PlayerHealthData healthData;

    void Start()
    {
        healthData.playerCurrentHealth = healthData.playerMaxHealth;
        Debug.Log("Starting health is: " + healthData.playerCurrentHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        healthData.playerCurrentHealth = Mathf.Clamp(healthData.playerCurrentHealth - damageAmount, 0, 100);
        Debug.Log("Health is: " + healthData.playerCurrentHealth);
    }
}
