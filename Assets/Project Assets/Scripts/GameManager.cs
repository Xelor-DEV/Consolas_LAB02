using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GameManager : MonoBehaviour
{
    [Header("Data Configuration")]
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private PlayerConfigSO playerConfig;

    [Header("Tank Prefabs")]
    [SerializeField] private GameObject tankPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("UI Manager")]
    public UIManager uiManager;

    [Header("Enemy Spawner")]
    public EnemySpawner enemySpawner;

    [Header("Game Result")]
    public GameResultSO gameResultSO;

    public UnityEvent OnWin;
    public UnityEvent OnLose;

    public List<TankManager> activeTanks = new List<TankManager>();
    private ReadOnlyArray<InputDevice> allDevices;
    private int remainingTanks;


    public PlayerConfigSO PlayerConfig { get; }
    // Singleton pattern para fácil acceso
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();

        if (playerConfig.isVersusMode == false)
        {
            SetupEventListeners();
        }
        else
        {
            remainingTanks = activeTanks.Count;
        }
    }

    public void OnTankDestroyed(int tankNumber)
    {
        if (playerConfig.isVersusMode == false) return;

        remainingTanks--;

        if (remainingTanks <= 1)
        {
            // Buscar el último tanque activo
            TankManager winner = activeTanks.FirstOrDefault(t => !t.IsDisabled);

            if (winner != null)
            {
                // Establecer la condición de victoria y el número del tanque ganador
                gameResultSO.victoryCondition = VictoryCondition.VersusWin;
                gameResultSO.winningTankNumber = winner.TankNumber; // Añadir esta línea

                // Disparar evento de victoria
                OnWin.Invoke();
            }
        }
    }

    private void SetupEventListeners()
    {
        enemySpawner.OnKillsUpdated.AddListener(UpdateKillsUI);
        enemySpawner.OnVictoryConditionMet.AddListener(OnVictoryConditionMet);
        UpdateKillsUI(enemySpawner.currentKills);
    }

    private void UpdateKillsUI(int currentKills)
    {
        uiManager.UpdateKillsDisplay(currentKills, enemySpawner.requiredKills);
    }

    public void OnTimeUp()
    {
        if (enemySpawner.currentKills >= enemySpawner.requiredKills)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    private void OnVictoryConditionMet()
    {
        WinGame();
    }

    private void WinGame()
    {
        UpdateGameResult(VictoryCondition.ExtraKills);
        OnWin.Invoke();
    }

    public void LoseGame()
    {
        UpdateGameResult(VictoryCondition.TimeOut);
        OnLose.Invoke();
    }

    private void UpdateGameResult(VictoryCondition condition)
    {
        gameResultSO.victoryCondition = condition;
        gameResultSO.enemiesKilled = enemySpawner.currentKills;
        gameResultSO.timeElapsed = uiManager.GetElapsedTime();
    }

    private void InitializeGame()
    {
        SpawnTanksBasedOnPlayerData();
        AssignDevicesToTanks();
    }

    private void SpawnTanksBasedOnPlayerData()
    {
        // Determinar cuántos tanques únicos tenemos en los datos
        var uniqueTankNumbers = playerData.playerSelections
            .Select(ps => ps.tankNumber)
            .Distinct()
            .ToList();

        int totalTanks = uniqueTankNumbers.Count;

        for (int i = 0; i < totalTanks; i++)
        {
            SpawnTank(uniqueTankNumbers[i], totalTanks, i);
        }

        Debug.Log($"Se crearon {totalTanks} tanques para {playerData.playerSelections.Count} jugadores");
    }

    private void SpawnTank(int tankNumber, int totalTanks, int tankIndex)
    {
        if (tankPrefab == null)
        {
            Debug.LogError("¡No hay prefab de tanque asignado!");
            return;
        }

        // Seleccionar punto de spawn (rotar si hay más tanques que puntos de spawn)
        Transform spawnPoint = spawnPoints[(tankNumber - 1) % spawnPoints.Length];

        // Instanciar tanque
        GameObject tankObj = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);
        TankManager tankManager = tankObj.GetComponent<TankManager>();

        if (tankManager != null)
        {
            // Obtener ambos jugadores para este tanque
            var driver = playerData.playerSelections.FirstOrDefault(ps => ps.tankNumber == tankNumber && ps.isDriver);
            var gunner = playerData.playerSelections.FirstOrDefault(ps => ps.tankNumber == tankNumber && !ps.isDriver);

            tankManager.InitializeTank(tankNumber, totalTanks, tankIndex, driver, gunner,
                                     playerConfig.isVersusMode, playerData.tankColors[tankNumber - 1]);
            activeTanks.Add(tankManager);
        }
        else
        {
            Debug.LogError("El prefab del tanque no tiene componente TankManager");
        }
    }

    private void AssignDevicesToTanks()
    {
        allDevices = InputSystem.devices;

        foreach (var selection in playerData.playerSelections)
        {
            // Encontrar el dispositivo por su ID
            InputDevice device = allDevices.FirstOrDefault(d => d.deviceId == selection.deviceId);

            if (device == null)
            {
                Debug.LogWarning($"Dispositivo no encontrado para Player {selection.playerNumber}");
                continue;
            }

            // Encontrar el tanque correspondiente
            TankManager tank = activeTanks.FirstOrDefault(t => t.TankNumber == selection.tankNumber);

            if (tank != null)
            {
                if (selection.isDriver)
                {
                    tank.AssignDriver(device);
                }
                else
                {
                    tank.AssignGunner(device);
                }
            }
            else
            {
                Debug.LogWarning($"Tanque {selection.tankNumber} no encontrado para Player {selection.playerNumber}");
            }
        }
    }

    public void CheckAllTanksManned()
    {
        bool allManned = activeTanks.All(tank => tank.IsFullyManned());

        if (allManned)
        {
            Debug.Log("¡Todos los tanques están completamente equipados! Iniciando juego...");
        }
        else
        {
            Debug.Log("Algunos tanques no tienen tripulación completa");
        }
    }

    // Manejar cambios de dispositivos
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
        if (change == InputDeviceChange.Reconnected)
        {
            ReconnectDevice(device);
        }
    }

    private void ReconnectDevice(InputDevice device)
    {
        // Buscar si este dispositivo estaba asignado a algún tanque
        foreach (var selection in playerData.playerSelections)
        {
            if (selection.deviceId == device.deviceId)
            {
                TankManager tank = activeTanks.FirstOrDefault(t => t.TankNumber == selection.tankNumber);

                if (tank != null)
                {
                    if (selection.isDriver)
                    {
                        tank.AssignDriver(device);
                    }
                    else
                    {
                        tank.AssignGunner(device);
                    }
                }
                break;
            }
        }
    }
}