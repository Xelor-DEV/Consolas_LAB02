using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretControl : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float pitchSpeed = 30f;
    [SerializeField] private float minPitchAngle = -10f;
    [SerializeField] private float maxPitchAngle = 30f;

    [Header("Ammo Settings")]
    [SerializeField] private int initialAmmo = 10;
    [SerializeField] private int ammoPerPowerUp = 5;
    [SerializeField] private bool unlimitedAmmo = false;

    [Header("Screen Shake")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TankManager tankManager;
    [SerializeField] private Transform cannon;
    [SerializeField] private TMP_Text ammo;
    [SerializeField] private PlayerConfigSO playerConfig;

    [Header("Vibration")]
    [SerializeField] private VibrationConfig vibrationConfig;

    private Vector2 aimInput;
    private float currentPitch = 0f;
    private int currentAmmo;
    private Gamepad gunnerGamepad;

    private CameraShake driverCameraShake;

    private void Start()
    {
        if (tankManager != null)
        {
            unlimitedAmmo = !playerConfig.isVersusMode;
        }

        currentAmmo = initialAmmo;
        UpdateAmmoUI();

        if (tankManager?.GunnerDevice is Gamepad)
        {
            gunnerGamepad = tankManager.GunnerDevice as Gamepad;
        }

        // Inicializar la configuración de vibración
        if (vibrationConfig != null)
        {
            vibrationConfig.Initialize(this);
        }

        if (tankManager != null && tankManager.GunnerCamera != null)
        {
            driverCameraShake = tankManager.GunnerCamera.GetComponent<CameraShake>();
            if (driverCameraShake == null)
                driverCameraShake = tankManager.GunnerCamera.gameObject.AddComponent<CameraShake>();
        }
    }


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
        if (tankManager?.IsDisabled == true) return;
        if (!unlimitedAmmo && currentAmmo <= 0) return;

        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

        if (driverCameraShake != null)
            driverCameraShake.TriggerShake(shakeDuration, shakeMagnitude);

        if (!unlimitedAmmo)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }

        // Aplicar vibración
        if (vibrationConfig != null && gunnerGamepad != null)
        {
            vibrationConfig.TriggerVibration(gunnerGamepad);
        }
    }

    private void OnDisable()
    {
        // Detener vibración si el objeto se desactiva
        if (gunnerGamepad != null && vibrationConfig != null)
        {
            vibrationConfig.StopVibration(gunnerGamepad);
        }
    }

    public void AddAmmo(int amount)
    {
        // Only add ammo in versus mode
        if (!unlimitedAmmo)
        {
            currentAmmo += amount;
            UpdateAmmoUI();
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammo != null && !unlimitedAmmo)
        {
            ammo.text = currentAmmo.ToString();
        }
    }

    void Update()
    {
        if (tankManager?.IsDisabled == true) return;

        // Rotación horizontal de la torreta (eje Y)
        transform.Rotate(Vector3.forward, aimInput.x * rotationSpeed * Time.deltaTime, Space.Self);

        // Rotación vertical del cañón (pitch - eje X local)
        if (cannon != null)
        {
            // Actualizar el ángulo de pitch con límites
            currentPitch -= aimInput.y * pitchSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitchAngle, maxPitchAngle);

            // Aplicar la rotación al cañón
            cannon.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        }
    }

    public int AmmoPerPowerUp
    {
        get
        {
            return ammoPerPowerUp;
        }
    }
    public bool HasUnlimitedAmmo
    {
        get { return unlimitedAmmo; }
    }
}