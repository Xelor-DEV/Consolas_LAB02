using UnityEngine;
using UnityEngine.InputSystem;

public class TurretControl : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 50f;

    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;

    private Vector2 aimInput;

    public void OnRotate(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, aimInput.x * rotationSpeed * Time.deltaTime, Space.Self);
    }
}