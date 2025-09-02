using UnityEngine;
using UnityEngine.InputSystem;

public class TankManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInput driverInput;
    [SerializeField] private PlayerInput gunnerInput;
    [SerializeField] private Camera driverCamera;
    [SerializeField] private Camera gunnerCamera;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private TankMovement tankMovement;
    [SerializeField] private TurretControl turretControl;

    [Header("Configuración")]
    [SerializeField] private int tankNumber;
    private bool isDisabled = false;

    private InputDevice driverDevice;
    private InputDevice gunnerDevice;

    public void InitializeTank(int tankNumber, int totalTanks, int tankIndex)
    {
        this.tankNumber = tankNumber;

        // Configurar cámaras según el número total de tanques
        SetupSplitScreen(totalTanks, tankIndex);

        // Configurar barra de vida según el número total de tanques
        SetupHealthBar(totalTanks, tankIndex);

        // Configurar audio listener (solo mantener uno)
        SetupAudioListener();
    }

    private void SetupSplitScreen(int totalTanks, int tankIndex)
    {
        if (driverCamera == null || gunnerCamera == null) return;

        // Configurar viewport rects según el número de tanques
        switch (totalTanks)
        {
            case 1:
                // Pantalla completa para un solo tanque
                driverCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                gunnerCamera.rect = new Rect(0, 0, 1, 0.5f);
                break;

            case 2:
                // Dos tanques - dividir horizontalmente
                float halfWidth = 0.5f;
                if (tankIndex == 0)
                {
                    driverCamera.rect = new Rect(0, 0.5f, halfWidth, 0.5f);
                    gunnerCamera.rect = new Rect(0, 0, halfWidth, 0.5f);
                }
                else
                {
                    driverCamera.rect = new Rect(halfWidth, 0.5f, halfWidth, 0.5f);
                    gunnerCamera.rect = new Rect(halfWidth, 0, halfWidth, 0.5f);
                }
                break;

            case 3:
                // Tres tanques - dividir en tres partes
                float thirdWidth = 0.333f;
                if (tankIndex == 0)
                {
                    driverCamera.rect = new Rect(0, 0.5f, thirdWidth, 0.5f);
                    gunnerCamera.rect = new Rect(0, 0, thirdWidth, 0.5f);
                }
                else if (tankIndex == 1)
                {
                    driverCamera.rect = new Rect(thirdWidth, 0.5f, thirdWidth, 0.5f);
                    gunnerCamera.rect = new Rect(thirdWidth, 0, thirdWidth, 0.5f);
                }
                else
                {
                    driverCamera.rect = new Rect(2 * thirdWidth, 0.5f, thirdWidth, 0.5f);
                    gunnerCamera.rect = new Rect(2 * thirdWidth, 0, thirdWidth, 0.5f);
                }
                break;
        }
    }

    private void SetupHealthBar(int totalTanks, int tankIndex)
    {
        if (healthBar == null) return;

        // Configurar posición de la barra de vida según el número de tanques
        switch (totalTanks)
        {
            case 1:
                // Barra en la izquierda para un solo tanque
                healthBar.anchorMin = new Vector2(0, 0.5f);
                healthBar.anchorMax = new Vector2(0, 0.5f);
                healthBar.pivot = new Vector2(0, 0.5f);
                healthBar.anchoredPosition = new Vector2(20, 0);
                break;

            case 2:
                if (tankIndex == 0)
                {
                    // Barra izquierda para el primer tanque
                    healthBar.anchorMin = new Vector2(0, 0.5f);
                    healthBar.anchorMax = new Vector2(0, 0.5f);
                    healthBar.pivot = new Vector2(0, 0.5f);
                    healthBar.anchoredPosition = new Vector2(20, 0);
                }
                else
                {
                    // Barra derecha para el segundo tanque
                    healthBar.anchorMin = new Vector2(1, 0.5f);
                    healthBar.anchorMax = new Vector2(1, 0.5f);
                    healthBar.pivot = new Vector2(1, 0.5f);
                    healthBar.anchoredPosition = new Vector2(-20, 0);
                }
                break;

            case 3:
                if (tankIndex == 0)
                {
                    // Barra izquierda para el primer tanque
                    healthBar.anchorMin = new Vector2(0, 0.5f);
                    healthBar.anchorMax = new Vector2(0, 0.5f);
                    healthBar.pivot = new Vector2(0, 0.5f);
                    healthBar.anchoredPosition = new Vector2(20, 0);
                }
                else if (tankIndex == 1)
                {
                    // Barra derecha para el segundo tanque
                    healthBar.anchorMin = new Vector2(1, 0.5f);
                    healthBar.anchorMax = new Vector2(1, 0.5f);
                    healthBar.pivot = new Vector2(1, 0.5f);
                    healthBar.anchoredPosition = new Vector2(-20, 0);
                }
                else
                {
                    // Barra superior rotada para el tercer tanque
                    healthBar.anchorMin = new Vector2(0.5f, 1);
                    healthBar.anchorMax = new Vector2(0.5f, 1);
                    healthBar.pivot = new Vector2(0.5f, 1);
                    healthBar.rotation = Quaternion.Euler(0, 0, -90);
                    healthBar.anchoredPosition = new Vector2(0, -20);
                }
                break;
        }
    }

    private void SetupAudioListener()
    {
        // Solo mantener un AudioListener en el primer tanque
        if (tankNumber > 1)
        {
            AudioListener driverAudio = driverCamera.GetComponent<AudioListener>();
            AudioListener gunnerAudio = gunnerCamera.GetComponent<AudioListener>();

            if (driverAudio != null) Destroy(driverAudio);
            if (gunnerAudio != null) Destroy(gunnerAudio);
        }
    }

    public void AssignDriver(InputDevice device)
    {
        driverDevice = device;
        if (driverInput != null)
        {
            driverInput.SwitchCurrentControlScheme(device);
            Debug.Log($"Asignado conductor al Tank {tankNumber}: {device.displayName}");
        }
    }

    public void AssignGunner(InputDevice device)
    {
        gunnerDevice = device;
        if (gunnerInput != null)
        {
            gunnerInput.SwitchCurrentControlScheme(device);
            Debug.Log($"Asignado artillero al Tank {tankNumber}: {device.displayName}");
        }
    }

    public bool IsFullyManned()
    {
        return driverDevice != null && gunnerDevice != null;
    }

    public void DisableTankControls()
    {
        isDisabled = true;

        if (tankMovement != null)
        {
            tankMovement.enabled = false;
        }

        if (turretControl != null)
        {
            turretControl.enabled = false;
        }

        CheckAllTanksDisabled();
    }


    private void CheckAllTanksDisabled()
    {
        // Buscar todos los tanques en escena
        TankManager[] allTanks = GameManager.Instance.activeTanks.ToArray();
        bool allDisabled = true;

        foreach (TankManager tank in allTanks)
        {
            if (!tank.isDisabled)
            {
                allDisabled = false;
                break;
            }
        }

        if (allDisabled)
        {
            // Activar derrota
            GameManager.Instance.LoseGame();
        }
    }


    // Propiedades para acceso externo
    public int TankNumber => tankNumber;
    public bool IsDisabled => isDisabled;
    public InputDevice DriverDevice => driverDevice;
    public InputDevice GunnerDevice => gunnerDevice;
    public PlayerInput DriverInput => driverInput;
    public PlayerInput GunnerInput => gunnerInput;
    public Camera DriverCamera => driverCamera;
    public Camera GunnerCamera => gunnerCamera;
    public RectTransform HealthBar => healthBar;
}