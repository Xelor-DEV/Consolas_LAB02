using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("UI de la vida")]
    public Image healthBar;

    [Header("Configuración de daño")]
    public float damageAmount = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount -= damageAmount;

            if (healthBar.fillAmount <= 0f)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("Enemigo muerto");
        Destroy(gameObject);
    }
}
