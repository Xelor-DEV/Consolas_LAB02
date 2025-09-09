using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text timerText;
    public TMP_Text killsText;
    public PlayerConfigSO playerConfig;

    [Header("Game Settings")]
    public float gameTime = 180f; // 3 minutos por defecto


    private float currentTime;
    private bool timerRunning = true;

    private void Start()
    {
        if (playerConfig.isVersusMode == true)
        {
            return;
        }

        currentTime = gameTime;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (playerConfig.isVersusMode == true)
        {
            return;
        }

        if (timerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                timerRunning = false;
                GameManager.Instance.OnTimeUp();
                
            }
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void UpdateKillsDisplay(int currentKills, int requiredKills)
    {
        killsText.text = $"{currentKills}/{requiredKills}";
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public float GetElapsedTime()
    {
        return gameTime - currentTime;
    }
}