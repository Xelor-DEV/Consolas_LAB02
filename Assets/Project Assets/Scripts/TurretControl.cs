using UnityEngine;
using UnityEngine.InputSystem;

public class TurretControl : MonoBehaviour
{
    public float horizontalSpeed = 50f;
    public float verticalSpeed = 30f;
    private Vector2 _aimInput;

    public GameObject aimPoint;
    public GameObject bullet;

    public void OnAim(InputAction.CallbackContext context)
    {
        _aimInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.performed == true)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Instantiate(bullet,aimPoint.transform.position, aimPoint.transform.rotation);
    }
    void Update()
    {
        // Rotación horizontal de la torreta
        transform.Rotate(Vector3.up, _aimInput.x * horizontalSpeed * Time.deltaTime, Space.Self);
        // Rotación vertical del cañón (ajusta según estructura)
        // Ej: transform.Rotate(Vector3.right, _aimInput.y * verticalSpeed * Time.deltaTime, Space.Self);
    }
}