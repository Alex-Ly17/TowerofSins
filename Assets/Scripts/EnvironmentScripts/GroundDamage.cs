using UnityEngine;
using static PlayerHealthManager;

public class GroundDamage : MonoBehaviour
{
    private BoxCollider myBoxCollider;
    public float onEntryDamage = 10f;
    public float onStayDamage = 1f;
    private float damageInterval = 2f;
    private float timeCheck = 0f;

    void Awake()
    {
        myBoxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
        playerHealth.TakeDamage(onEntryDamage);
    }

    private void OnTriggerStay(Collider other)
    {
        timeCheck += Time.deltaTime;
        if (timeCheck >= damageInterval)
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            playerHealth.TakeDamage(onStayDamage);
            timeCheck = 0;
        }
        
    }
}
