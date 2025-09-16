using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed = 20f; // Velocidad del proyectil
    [SerializeField] private string objective;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosionPrefab;

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
            DestroyBullet();
        }
        else if (other.tag == "ground")
        {
            DestroyBullet();
        }
        else if (other.tag == "wall")
        {
            Destroy(other.gameObject);
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}