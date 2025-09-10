using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSelection
{
    public int deviceId;
    public int playerNumber;
    public int tankNumber;
    public bool isDriver; // true = driver, false = gunner
    public Color playerColor;
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public List<PlayerSelection> playerSelections = new List<PlayerSelection>();

    public Color[] tankColors;

    public void ResetData()
    {
        playerSelections.Clear();
    }

    public void AddPlayerSelection(int deviceId, int playerNumber, int tankNumber, bool isDriver, Color playerColor)
    {
        // Remove any existing selection for this player
        playerSelections.RemoveAll(ps => ps.playerNumber == playerNumber);

        // Add new selection
        playerSelections.Add(new PlayerSelection
        {
            deviceId = deviceId,
            playerNumber = playerNumber,
            tankNumber = tankNumber,
            isDriver = isDriver,
            playerColor = playerColor
        });
    }
}