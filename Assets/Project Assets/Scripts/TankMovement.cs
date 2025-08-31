using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 30f;

    [Header("Component References")]
    [SerializeField] private Rigidbody tankRigidbody;

    private Vector2 moveInput;

    private void Awake()
    {
        if (tankRigidbody == null)
        {
            tankRigidbody = GetComponent<Rigidbody>();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.forward * (moveInput.y * moveSpeed) * Time.fixedDeltaTime;
        tankRigidbody.MovePosition(tankRigidbody.position + moveDirection);
    }

    private void HandleRotation()
    {
        float rotation = (moveInput.x * turnSpeed) * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        tankRigidbody.MoveRotation(tankRigidbody.rotation * turnRotation);
    }
}