using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text messageText;
    public TMP_Text killsText;
    public TMP_Text timeText;

    [Header("Messages")]
    public string timeOutMessage = "Time's Up! Not enough kills.";
    public string enoughKillsMessage = "Mission Accomplished!";
    public string extraKillsMessage = "Excellent Performance!";

    public void DisplayResults(GameResultSO result)
    {
        switch (result.victoryCondition)
        {
            case VictoryCondition.TimeOut:
                messageText.text = timeOutMessage;
                break;
            case VictoryCondition.EnoughKills:
                messageText.text = enoughKillsMessage;
                break;
            case VictoryCondition.ExtraKills:
                messageText.text = extraKillsMessage;
                break;
        }

        killsText.text = $"Kills: {result.enemiesKilled}";

        int minutes = Mathf.FloorToInt(result.timeElapsed / 60);
        int seconds = Mathf.FloorToInt(result.timeElapsed % 60);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }
}