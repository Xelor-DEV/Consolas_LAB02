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

    [Header("Vibration")]
    [SerializeField] private MovementVibrationConfig movementVibrationConfig;

    private Vector2 moveInput;
    private float originalMoveSpeed;
    private Coroutine speedBoostCoroutine;
    private Gamepad driverGamepad;
    private Coroutine vibrationCoroutine;
    private float currentLowFreq;
    private float currentHighFreq;

    private void Awake()
    {
        if (tankRigidbody == null)
        {
            tankRigidbody = GetComponent<Rigidbody>();
        }

        originalMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        // Obtener el Gamepad del conductor
        if (tankManager?.DriverDevice is Gamepad)
        {
            driverGamepad = tankManager.DriverDevice as Gamepad;
        }

        // Iniciar la corrutina de vibración
        if (driverGamepad != null && movementVibrationConfig != null)
        {
            vibrationCoroutine = StartCoroutine(VibrationUpdateRoutine());
        }
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

    private IEnumerator VibrationUpdateRoutine()
    {
        while (true)
        {
            if (driverGamepad != null && movementVibrationConfig != null)
            {
                bool isMoving = moveInput.magnitude > 0.1f;

                // Obtener los valores objetivo de vibración
                var targetVibration = movementVibrationConfig.GetVibrationValues(isMoving, isRunning);

                // Suavizar la transición
                currentLowFreq = Mathf.Lerp(currentLowFreq, targetVibration.low,
                    Time.deltaTime * movementVibrationConfig.transitionSmoothness);
                currentHighFreq = Mathf.Lerp(currentHighFreq, targetVibration.high,
                    Time.deltaTime * movementVibrationConfig.transitionSmoothness);

                // Aplicar al gamepad
                driverGamepad.SetMotorSpeeds(currentLowFreq, currentHighFreq);
            }

            yield return null;
        }
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

    private void OnDisable()
    {
        // Detener vibración si el objeto se desactiva
        if (driverGamepad != null)
        {
            driverGamepad.SetMotorSpeeds(0f, 0f);
        }

        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
        }
    }
}