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
        // Rotaci�n horizontal de la torreta
        transform.Rotate(Vector3.up, _aimInput.x * horizontalSpeed * Time.deltaTime, Space.Self);
        // Rotaci�n vertical del ca��n (ajusta seg�n estructura)
        // Ej: transform.Rotate(Vector3.right, _aimInput.y * verticalSpeed * Time.deltaTime, Space.Self);
    }
}