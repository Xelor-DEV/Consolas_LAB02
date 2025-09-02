using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCountSelector : MonoBehaviour
{
    [SerializeField] private PlayerConfigSO playerConfig;

    public void SetMaxPlayers(int optionIndex)
    {
        MaxPlayersOption option = (MaxPlayersOption)optionIndex;
        SetMaxPlayers(option);
    }

    public void SetMaxPlayers(MaxPlayersOption option)
    {
        playerConfig.maxPlayers = option;
        Debug.Log($"Modo seleccionado: {option}");

        // Cargar siguiente escena
        SceneManager.LoadScene("Menu");
    }
}