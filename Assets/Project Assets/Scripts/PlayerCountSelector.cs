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

        // Validar selección según el modo
        if (playerConfig.isVersusMode)
        {
            if (compatibleDevices < 4)
            {
                Debug.LogWarning("Versus mode requires at least 4 compatible devices");
                return;
            }

            if (option == MaxPlayersOption.Max2Players)
            {
                Debug.LogWarning("Versus mode requires at least 4 players (2 tanks)");
                return;
            }
        }
        else // Cooperative mode
        {
            int requiredPlayers = GetRequiredPlayers(option, compatibleDevices);
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