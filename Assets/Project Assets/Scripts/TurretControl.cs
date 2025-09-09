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

    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TankManager tankManager;
    [SerializeField] private Transform cannon;
    [SerializeField] private TMP_Text ammo;

    private Vector2 aimInput;
    private float currentPitch = 0f;
    private int currentAmmo;

    private void Start()
    {
        currentAmmo = initialAmmo;
        UpdateAmmoUI();
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
        if (currentAmmo <= 0) return;

        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        currentAmmo--;
        UpdateAmmoUI();
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        UpdateAmmoUI();
    }
    private void UpdateAmmoUI()
    {
        ammo.text = currentAmmo.ToString();
    }


    void Update()
    {
        if (tankManager?.IsDisabled == true) return;

        // Rotaci�n horizontal de la torreta (eje Y)
        transform.Rotate(Vector3.forward, aimInput.x * rotationSpeed * Time.deltaTime, Space.Self);

        // Rotaci�n vertical del ca��n (pitch - eje X local)
        if (cannon != null)
        {
            // Actualizar el �ngulo de pitch con l�mites
            currentPitch += aimInput.y * pitchSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitchAngle, maxPitchAngle);

            // Aplicar la rotaci�n al ca��n
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
}