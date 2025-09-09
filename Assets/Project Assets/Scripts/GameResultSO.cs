using UnityEngine;

public enum VictoryCondition
{
    TimeOut,
    EnoughKills,
    ExtraKills,
    VersusWin
}

[CreateAssetMenu(fileName = "GameResultSO", menuName = "Game/Game Result")]
public class GameResultSO : ScriptableObject
{
    public VictoryCondition victoryCondition;
    public int enemiesKilled;
    public float timeElapsed;
    public int winningTankNumber;
}