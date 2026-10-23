using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Scriptable Object Referece")]
    public PlayerHealthData healthData;
    public UIDocument UIDoc;

    private Label m_HealthLabel;
    private VisualElement m_HealthBarMask;

    void Start()
    {
        // declare health amount + bar UI
        m_HealthLabel = UIDoc.rootVisualElement.Q<Label>("HealthLabel");
        m_HealthBarMask = UIDoc.rootVisualElement.Q<VisualElement>("HealthBarMask");

        // set up initial health ui (100 / 100)
        m_HealthLabel.text = $"{healthData.playerCurrentHealth}/{healthData.playerMaxHealth}";

        healthData.playerCurrentHealth = healthData.playerMaxHealth;

        // set up health bar 
        float healthRatio = (float)healthData.playerCurrentHealth / healthData.playerMaxHealth;
        float healthPercent = Mathf.Lerp(8, 88, healthRatio);
        m_HealthBarMask.style.width = Length.Percent(healthPercent);
    }

    public void TakeDamage(float damageAmount)
    {
        // take amount of damage
        healthData.playerCurrentHealth = Mathf.Clamp(healthData.playerCurrentHealth - damageAmount, 0, 100);

        // health UI update
        m_HealthLabel.text = $"{healthData.playerCurrentHealth}/{healthData.playerMaxHealth}";

        // health bar update
        float healthRatio = (float)healthData.playerCurrentHealth / healthData.playerMaxHealth;
        float healthPercent = Mathf.Lerp(8, 88, healthRatio);
        m_HealthBarMask.style.width = Length.Percent(healthPercent);
    }
}
