using UnityEngine;
using Debug = UnityEngine.Debug;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Push full state to UI on initialization frame
        UIManager.Instance?.UpdateHealthBar(GetHealthPercent());
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! HP: {currentHealth}/{maxHealth}");

        // Update the health display bar
        UIManager.Instance?.UpdateHealthBar(GetHealthPercent());

        if (currentHealth <= 0)
            Die();
    }

    /// Restores health up to the maximum capped limit.
    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Health already full! Cannot consume Herb.");
            UIManager.Instance?.ShowPickupPrompt("Health already full!");
            return;
        }

        currentHealth += amount;

        // Clamp health so it never overflows maxHealth (e.g., 105/100)
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} healed by {amount}! Current HP: {currentHealth}/{maxHealth}");

        // UPDATE HEALTH BAR VISUAL
        UIManager.Instance?.UpdateHealthBar(GetHealthPercent());
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }

    public int GetHealth() => currentHealth;
    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}