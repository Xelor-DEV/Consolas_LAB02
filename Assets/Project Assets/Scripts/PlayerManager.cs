using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum MaxPlayersOption
{
    Max2Players,
    Max4Players,
    Max6Players,
    AllDevices
}

public class PlayerManager : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerCursorPrefab;
    [SerializeField] private GameObject tankSelectorPrefab;

    [Header("UI Configuration")]
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform canvas;

    [Header("Player Configuration")]
    [SerializeField] private PlayerConfigSO playerConfig;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    [Header("Player Colors")]
    [SerializeField]
    private Color[] playerColors = new Color[] {
        Color.red, Color.blue, Color.green, Color.yellow,
        Color.cyan, Color.magenta
    };

    private List<InputDevice> compatibleDevices = new List<InputDevice>();
    private List<TankSelector> tankSelectors = new List<TankSelector>();
    private List<PlayerCursor> playerCursors = new List<PlayerCursor>();
    private List<RectTransform> allButtonPositions = new List<RectTransform>();

    private ReadOnlyArray<InputControlScheme> controlSchemes;
    private int totalPlayers;
    private int playersReady;

    // Singleton para fácil acceso
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        controlSchemes = inputActions.controlSchemes;
        playerDataSO.ResetData();
        DetectCompatibleDevices();
        CreateTankSelectors();    // Primero creamos los selectores de tanques
        SetupAllButtonPositions(); // Luego configuramos las posiciones de los botones
        CreatePlayerCursors();    // Finalmente creamos los cursores con las posiciones ya disponibles
    }

    private void DetectCompatibleDevices()
    {
        compatibleDevices.Clear();

        // Obtener todos los dispositivos conectados
        var allDevices = InputSystem.devices;

        foreach (var device in allDevices)
        {
            // Verificar si el dispositivo es compatible con algún control scheme
            foreach (InputControlScheme scheme in controlSchemes)
            {
                if (scheme.SupportsDevice(device))
                {
                    compatibleDevices.Add(device);
                    break;
                }
            }
        }

        int maxPlayers = GetMaxPlayers();

        if (maxPlayers > 0 && compatibleDevices.Count > maxPlayers)
        {
            compatibleDevices.RemoveRange(maxPlayers, compatibleDevices.Count - maxPlayers);
        }

        if (compatibleDevices.Count % 2 != 0 && compatibleDevices.Count > 0)
        {
            compatibleDevices.RemoveAt(compatibleDevices.Count - 1);
        }

        if (compatibleDevices.Count < 2)
        {
            Debug.LogError("¡No hay suficientes dispositivos compatibles! Se requieren al menos 2.");
            return;
        }

        totalPlayers = compatibleDevices.Count;
        Debug.Log($"Jugadores detectados: {totalPlayers}");
    }

    private int GetMaxPlayers()
    {
        if (playerConfig == null) return 0;

        switch (playerConfig.maxPlayers)
        {
            case MaxPlayersOption.Max2Players:
                return 2;
            case MaxPlayersOption.Max4Players:
                return 4;
            case MaxPlayersOption.Max6Players:
                return 6;
            default:
                return 0; // 0 = sin límite
        }
    }

    private void CreateTankSelectors()
    {
        int numberOfTanks = totalPlayers / 2;

        for (int i = 0; i < numberOfTanks; i++)
        {
            GameObject tankSelectorObj = Instantiate(tankSelectorPrefab, container);
            TankSelector tankSelector = tankSelectorObj.GetComponent<TankSelector>();
            tankSelector.Initialize(i + 1);
            tankSelectors.Add(tankSelector);
        }
    }

    private void CreatePlayerCursors()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            GameObject cursorObj = Instantiate(playerCursorPrefab, canvas);
            PlayerCursor playerCursor = cursorObj.GetComponent<PlayerCursor>();

            // Configurar el cursor
            playerCursor.Initialize(
                device: compatibleDevices[i],
                playerNumber: i + 1,
                playerColor: playerColors[i % playerColors.Length]
            );

            playerCursors.Add(playerCursor);
        }
    }

    private void SetupAllButtonPositions()
    {
        allButtonPositions.Clear();

        foreach (TankSelector tankSelector in tankSelectors)
        {
            allButtonPositions.Add(tankSelector.TankButtonRect);
            allButtonPositions.Add(tankSelector.TurretButtonRect);
        }
    }

    public List<RectTransform> GetAllButtonPositions()
    {
        return allButtonPositions;
    }

    public bool TrySelectPosition(int buttonIndex, int playerNumber, out int tankNumber, out bool isDriver)
    {
        tankNumber = (buttonIndex / 2) + 1;
        isDriver = (buttonIndex % 2) == 0;

        // Verificar si esta posición ya está ocupada
        TankSelector tankSelector = tankSelectors[tankNumber - 1];

        if (isDriver && tankSelector.IsDriverSelected)
        {
            return false;
        }

        if (!isDriver && tankSelector.IsGunnerSelected)
        {
            return false;
        }

        // Marcar la posición como ocupada
        if (isDriver)
        {
            tankSelector.SetDriverSelected();
        }
        else
        {
            tankSelector.SetGunnerSelected();
        }

        // Guardar en ScriptableObject
        playerDataSO.AddPlayerSelection(
            playerCursors[playerNumber - 1].DeviceId,
            playerNumber,
            tankNumber,
            isDriver
        );

        return true;
    }

    public void PlayerReady()
    {
        playersReady++;

        if (playersReady == totalPlayers)
        {
            // Todos los jugadores están listos, cargar la escena
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        Debug.Log("Todos los jugadores están listos. Cargando escena de juego...");
        // Aquí cargarías tu escena de juego

        if (playerConfig.isVersusMode == true)
        {
            SceneManager.LoadScene("Versus");
        }
        else
        {
            SceneManager.LoadScene("Test");
        }
        
    }

    // Método para manejar cambios en dispositivos conectados
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            // Reiniciar la detección cuando hay cambios en dispositivos
            Debug.Log("Dispositivo cambiado, reiniciando detección...");
            RestartDetection();
        }
    }

    private void RestartDetection()
    {
        // Limpiar selectores existentes
        foreach (TankSelector selector in tankSelectors)
        {
            Destroy(selector.gameObject);
        }
        tankSelectors.Clear();

        foreach (PlayerCursor cursor in playerCursors)
        {
            Destroy(cursor.gameObject);
        }
        playerCursors.Clear();

        allButtonPositions.Clear();
        playersReady = 0;

        // Volver a detectar y crear selectores
        DetectCompatibleDevices();
        CreateTankSelectors();
        CreatePlayerCursors();
        SetupAllButtonPositions();
    }
}