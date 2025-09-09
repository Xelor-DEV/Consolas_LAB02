using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private bool isDead = false;

    [Header("Events")]
    [SerializeField] private UnityEvent OnDeath;
    [SerializeField] private UnityEvent OnDamage;

    public bool IsDead
    {
        get 
        { 
            return isDead; 
        }
    }

    private void Start()
    {
        // Setear la vida actual al máximo al inicio
        currentHealth = maxHealth;
        // Actualizar la barra de vida con el valor máximo
        UpdateHealthBar();
    }

    public virtual void Damage(float damage)
    {
        // Reducir la vida actual
        currentHealth -= damage;

        // Asegurarse de que la vida no sea negativa
        currentHealth = Mathf.Max(currentHealth, 0);

        // Invocar el evento de daño
        OnDamage?.Invoke();

        // Verificar si la vida llega a cero
        if (currentHealth <= 0)
        {
            // Invocar el evento de muerte
            OnDeath?.Invoke();
            isDead = true;
        }
    }

    public void UpdateHealthBar()
    {
        // Actualizar el fillAmount de la imagen de la barra de vida
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    public virtual void Death()
    {
        Destroy(gameObject);
    }
}