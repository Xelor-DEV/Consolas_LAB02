using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Velocidad del proyectil

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ground")
        {
            Destroy(gameObject);
        }
    }
}