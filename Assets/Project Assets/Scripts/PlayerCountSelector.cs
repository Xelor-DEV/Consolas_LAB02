using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

public class PlayerCountSelector : MonoBehaviour
{
    [SerializeField] private PlayerConfigSO playerConfig;
    [SerializeField] private TMP_Text modeButtonText;
    [SerializeField] private InputActionAsset inputActionAsset;

    private void Start()
    {
        UpdateModeButtonText();
    }

    public void ToggleGameMode()
    {
        playerConfig.isVersusMode = !playerConfig.isVersusMode;
        UpdateModeButtonText();
    }

    private void UpdateModeButtonText()
    {
        modeButtonText.text = playerConfig.isVersusMode ? "Versus" : "Cooperative";
    }

    public void SetMaxPlayers(int optionIndex)
    {
        MaxPlayersOption option = (MaxPlayersOption)optionIndex;

        // Obtener dispositivos compatibles
        int compatibleDevices = CountCompatibleDevices();
        int requiredPlayers = GetRequiredPlayers(option, compatibleDevices);

        // Validar selección según el modo
        if (playerConfig.isVersusMode)
        {
            // Versus mode requiere múltiplos de 2 (cada tanque necesita 2 jugadores)
            if (requiredPlayers % 2 != 0)
            {
                Debug.LogWarning("Versus mode requires an even number of players (each tank needs 2 players)");
                return;
            }

            // Versus mode requiere al menos 2 tanques (4 jugadores)
            if (requiredPlayers < 4)
            {
                Debug.LogWarning("Versus mode requires at least 4 players (2 tanks)");
                return;
            }

            // Verificar si hay suficientes dispositivos
            if (compatibleDevices < requiredPlayers)
            {
                Debug.LogWarning($"Not enough compatible devices for versus mode. Required: {requiredPlayers}, Found: {compatibleDevices}");
                return;
            }
        }
        else // Cooperative mode
        {
            // Verificar si hay suficientes dispositivos
            if (compatibleDevices < requiredPlayers)
            {
                Debug.LogWarning($"Not enough compatible devices. Required: {requiredPlayers}, Found: {compatibleDevices}");
                return;
            }
        }

        SetMaxPlayers(option);
    }

    private int CountCompatibleDevices()
    {
        HashSet<InputDevice> compatibleDevices = new HashSet<InputDevice>();

        foreach (InputControlScheme controlScheme in inputActionAsset.controlSchemes)
        {
            // Obtener dispositivos compatibles con este esquema de control
            var devices = InputSystem.devices.Where(device =>
                controlScheme.SupportsDevice(device));

            foreach (InputDevice device in devices)
            {
                compatibleDevices.Add(device);
            }
        }

        return compatibleDevices.Count;
    }

    private int GetRequiredPlayers(MaxPlayersOption option, int compatibleDevices)
    {
        return option switch
        {
            MaxPlayersOption.Max2Players => 2,
            MaxPlayersOption.Max4Players => 4,
            MaxPlayersOption.Max6Players => 6,
            MaxPlayersOption.AllDevices => compatibleDevices,
            _ => 0
        };
    }

    public void SetMaxPlayers(MaxPlayersOption option)
    {
        playerConfig.maxPlayers = option;
        Debug.Log($"Mode selected: {(playerConfig.isVersusMode ? "Versus" : "Cooperative")}, Players: {option}");

        // Load next scene
        SceneManager.LoadScene("Menu");
    }
}