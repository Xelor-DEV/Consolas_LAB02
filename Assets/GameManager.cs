using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    /*
    [Header("References")]
    public PlayerInputManager inputManager;
    private GameObject currentTank;

    [Header("Player Configuration")]
    private int currentPlayerCount = 0;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        currentPlayerCount++;

        // Los Impares seran los drivers y los Pares seran los gunners
        if (currentPlayerCount % 2 != 0)
        {
            ConfigureDriver(playerInput);
        }
        else
        {
            ConfigureGunner(playerInput);
        }

        if (currentPlayerCount >= inputManager.maxPlayerCount)
        {
            inputManager.DisableJoining();
        }
    }

    private void ConfigureDriver(PlayerInput playerInput)
    {
        playerInput.SwitchCurrentActionMap("Driver");

        // Configuramos el control de movimiento
        TankMovement movement = currentTank.GetComponent<TankMovement>();
        if (movement != null)
        {
            movement.SetDriverInput(playerInput);
        }

        // Activamos la c�mara del conductor
        // (asumiendo que tienes una referencia a esta c�mara)
        Camera driverCamera = currentTank.transform.Find("C�maraConductor").GetComponent<Camera>();
        driverCamera.enabled = true;
    }

    private void ConfigureGunner(PlayerInput playerInput)
    {
        playerInput.SwitchCurrentActionMap("Gunner");

        // Configuramos el control de la torreta
        TurretControl turret = currentTank.GetComponentInChildren<TurretControl>();
        if (turret != null)
        {
            turret.SetGunnerInput(playerInput);
        }

        // Activamos la c�mara del artillero
        Camera gunnerCamera = currentTank.transform.Find("C�maraArtillero").GetComponent<Camera>();
        gunnerCamera.enabled = true;
    }

    private void SetupSplitScreen()
    {
        // Configuraci�n autom�tica de pantalla dividida
        if (currentPlayerCount == 2)
        {
            // Para 2 jugadores: split vertical
            Camera[] cameras = currentTank.GetComponentsInChildren<Camera>();
            if (cameras.Length >= 2)
            {
                cameras[0].rect = new Rect(0, 0, 0.5f, 1); // Mitad izquierda
                cameras[1].rect = new Rect(0.5f, 0, 0.5f, 1); // Mitad derecha
            }
        }
        // Puedes agregar m�s configuraciones para 3-4 jugadores
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        currentPlayerCount--;
        playerInputManager.EnableJoining();

        // Reconfiguramos las c�maras
        SetupSplitScreen();
    }
    */
}