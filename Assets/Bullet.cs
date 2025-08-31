using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed = 20f; // Velocidad del proyectil
    [SerializeField] private string objective;
    [SerializeField] private float damage;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == objective)
        {
            HealthManager health = other.GetComponent<HealthManager>();
            health.Damage(damage);
            Destroy(gameObject);
        }
        else if (other.tag == "ground")
        {
            Destroy(gameObject);
        }
    }
}