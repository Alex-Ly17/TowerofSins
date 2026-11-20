using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    [Header("Scriptable Object Referece")]
    public EnemyHealthData enemyHealthData;
    private FlashOnHit flashOnHit;

    // for hit audio
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSfx;

    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        enemyHealthData.enemyCurrentHealth = enemyHealthData.enemyMaxHealth;
        healthSlider.value = enemyHealthData.enemyCurrentHealth / enemyHealthData.enemyMaxHealth;

        flashOnHit = GetComponent<FlashOnHit>();

        audioSource = GetComponent<AudioSource>();
    }

    public void EnemyTakeDamage(float damageAmount)
    {
        // take amount of damage
        enemyHealthData.enemyCurrentHealth = Mathf.Clamp(enemyHealthData.enemyCurrentHealth - damageAmount, 0, 100);
        healthSlider.value = enemyHealthData.enemyCurrentHealth / enemyHealthData.enemyMaxHealth;

        if (flashOnHit != null)
        {
            flashOnHit.OnHit();
        }

        audioSource.PlayOneShot(hitSfx);
    }
}
