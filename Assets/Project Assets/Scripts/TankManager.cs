using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField] private TMP_Text playerNumber_Driver;
    [SerializeField] private TMP_Text tankNumber_Driver;

    [SerializeField] private TMP_Text playerNumber_Gunner;
    [SerializeField] private TMP_Text tankNumber_Gunner;

    [SerializeField] private RectTransform dataContainerDriver;
    [SerializeField] private RectTransform dataContainerGunner;

    [Header("Configuración")]
    [SerializeField] private int tankNumber;
    private bool isDisabled = false;

    private InputDevice driverDevice;
    private InputDevice gunnerDevice;

    [Header("Trayectoria de Proyectil")]
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private int trajectoryResolution = 30;
    [SerializeField] private float trajectoryDuration = 2f;

    private LineRenderer trajectoryLine;
    private void Awake()
    {
        SetupLaunchPoint();
        SetupTrajectoryLine();
    }

    private void ApplyTankColor(PlayerSelection driver, PlayerSelection gunner, Color tankColor)
    {
        // Para el conductor (driver)
        if (driver != null)
        {
            if (playerNumber_Driver != null)
            {
                playerNumber_Driver.text = "Player " + driver.playerNumber.ToString();
                playerNumber_Driver.color = driver.playerColor;
            }

            if (tankNumber_Driver != null)
            {
                tankNumber_Driver.text = "Tank " + driver.tankNumber.ToString();
                tankNumber_Driver.color = tankColor;
            }
        }

        // Para el artillero (gunner)
        if (gunner != null)
        {
            if (playerNumber_Gunner != null)
            {
                playerNumber_Gunner.text = "Player " + gunner.playerNumber.ToString();
                playerNumber_Gunner.color = gunner.playerColor;
            }

            if (tankNumber_Gunner != null)
            {
                tankNumber_Gunner.text = "Tank " + gunner.tankNumber.ToString();
                tankNumber_Gunner.color = tankColor;
            }
        }
    }

    private void SetupLaunchPoint()
    {
        if (launchPoint == null)
        {
            Transform found = transform.Find("Muzzle");
            if (found == null)
            {
                // Si no está directamente como hijo, busca en toda la jerarquía
                found = GetComponentsInChildren<Transform>()
                    .FirstOrDefault(t => t.name == "Muzzle");
            }

            if (found != null)
            {
                launchPoint = found;
            }
            else
            {
                Debug.LogWarning($"[Tank {tankNumber}] No se encontró el punto de disparo 'Muzzle' en el prefab.");
            }
        }
    }
    private void SetupTrajectoryLine()
    {
        GameObject lineObj = new GameObject("TrajectoryLine");
        lineObj.transform.SetParent(transform);
        trajectoryLine = lineObj.AddComponent<LineRenderer>();
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.widthMultiplier = 0.1f;
        trajectoryLine.positionCount = trajectoryResolution;
        trajectoryLine.startColor = Color.yellow;
        trajectoryLine.endColor = Color.red;
    }
    private void DrawTrajectory()
    {
        if (trajectoryLine == null || launchPoint == null) return;

        Vector3[] points = new Vector3[trajectoryResolution];
        Vector3 startPosition = launchPoint.position;
        Vector3 startVelocity = launchPoint.forward * projectileSpeed;

        float timestep = trajectoryDuration / trajectoryResolution;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i * timestep;
            Vector3 point = startPosition + startVelocity * t + 0.5f * Physics.gravity * t * t;
            points[i] = point;
        }

        trajectoryLine.SetPositions(points);
    }
    private void Update()
    {
        DrawTrajectory(); // Puedes condicionar esto si solo quieres mostrarlo al apuntar
    }

    public void InitializeTank(int tankNumber, int totalTanks, int tankIndex,
                             PlayerSelection driver, PlayerSelection gunner,
                             bool isVersusMode, Color tankColor)
    {
        this.tankNumber = tankNumber;
        ApplyTankColor(driver, gunner, tankColor);
        SetupSplitScreen(totalTanks, tankIndex);
        SetupHealthBar(totalTanks, tankIndex);
        SetupDataContainers(totalTanks, tankIndex);
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

    private void SetupDataContainers(int totalTanks, int tankIndex)
    {
        if (dataContainerDriver == null || dataContainerGunner == null) return;

        // Para el driver (siempre arriba) y gunner (siempre abajo)
        // Pero posicionados según el número total de tanques y el índice
        switch (totalTanks)
        {
            case 1:
                SetDataContainerPosition(dataContainerDriver, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
                SetDataContainerPosition(dataContainerGunner, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
                break;
            case 2:
                if (tankIndex == 0)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
                }
                else
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 20));
                }
                break;
            case 3:
                if (tankIndex == 0)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
                }
                else if (tankIndex == 1)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 20));
                }
                else
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 20));
                }
                break;
            case 4:
                if (tankIndex == 0)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
                }
                else if (tankIndex == 1)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 20));
                }
                else if (tankIndex == 2)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 20));
                }
                else
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, 0));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0));
                }
                break;
            case 5:
                if (tankIndex == 0)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
                }
                else if (tankIndex == 1)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 20));
                }
                else if (tankIndex == 2)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 20));
                }
                else if (tankIndex == 3)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, 0));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0));
                }
                else
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-20, 0));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0));
                }
                break;
            case 6:
                if (tankIndex == 0)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
                }
                else if (tankIndex == 1)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 20));
                }
                else if (tankIndex == 2)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 20));
                }
                else if (tankIndex == 3)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, 0));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0));
                }
                else if (tankIndex == 4)
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-20, 0));
                }
                else
                {
                    SetDataContainerPosition(dataContainerDriver, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, 0));
                    SetDataContainerPosition(dataContainerGunner, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-20, 0));
                }
                break;
        }
    }

    private void SetDataContainerPosition(RectTransform container, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition)
    {
        container.anchorMin = anchorMin;
        container.anchorMax = anchorMax;
        container.pivot = pivot;
        container.anchoredPosition = anchoredPosition;
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