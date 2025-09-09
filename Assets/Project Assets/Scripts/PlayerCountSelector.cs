using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCountSelector : MonoBehaviour
{
    [SerializeField] private PlayerConfigSO playerConfig;
    [SerializeField] private TMP_Text modeButtonText;

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

        // Validate selection for versus mode
        if (playerConfig.isVersusMode && option == MaxPlayersOption.Max2Players)
        {
            Debug.LogWarning("Versus mode requires at least 4 players (2 tanks)");
            return;
        }

        SetMaxPlayers(option);
    }

    public void SetMaxPlayers(MaxPlayersOption option)
    {
        playerConfig.maxPlayers = option;
        Debug.Log($"Mode selected: {(playerConfig.isVersusMode ? "Versus" : "Cooperative")}, Players: {option}");

        // Load next scene
        SceneManager.LoadScene("Menu");
    }
}