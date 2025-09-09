using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    public MaxPlayersOption maxPlayers = MaxPlayersOption.AllDevices;
    public bool isVersusMode = false;
}