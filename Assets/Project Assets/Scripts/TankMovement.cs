using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TankMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float turnSpeed = 30f;
    public bool isRunning = false;

    [Header("PowerUp Settings")]
    [SerializeField] private float speedBoostDuration = 5f;

    [Header("Component References")]
    [SerializeField] private Rigidbody tankRigidbody;
    [SerializeField] private TankManager tankManager;

    private Vector2 moveInput;
    private float originalMoveSpeed;
    private Coroutine speedBoostCoroutine;

    private void Awake()
    {
        if (tankRigidbody == null)
        {
            tankRigidbody = GetComponent<Rigidbody>();
        }

        originalMoveSpeed = moveSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (tankManager?.IsDisabled == true) return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        Vector3 moveDirection = transform.forward * (moveInput.y * currentSpeed) * Time.fixedDeltaTime;
        tankRigidbody.MovePosition(tankRigidbody.position + moveDirection);
    }

    private void HandleRotation()
    {
        float rotation = (moveInput.x * turnSpeed) * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        tankRigidbody.MoveRotation(tankRigidbody.rotation * turnRotation);
    }

    public void ActivateSpeedBoost()
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }
        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine());
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        isRunning = true;
        yield return new WaitForSeconds(speedBoostDuration);
        isRunning = false;
    }
}