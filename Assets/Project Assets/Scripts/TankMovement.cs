using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 30f;
    private Vector2 _moveInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Movimiento hacia adelante/atrás
        transform.Translate(Vector3.forward * _moveInput.y * moveSpeed * Time.fixedDeltaTime);
        // Rotación izquierda/derecha
        transform.Rotate(Vector3.up, _moveInput.x * turnSpeed * Time.fixedDeltaTime);
    }
}