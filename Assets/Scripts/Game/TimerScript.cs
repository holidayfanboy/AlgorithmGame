using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float timeLimit;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] GameManager gameManager;
    public FirstStageData firstStageData;
    public bool startTimer;
    public bool timeisUp;
    public bool timerRunning;
    public bool isTimerFrozen; // For TimeFreeze skill
    private Color originalColor;
    private Color iceColor = new Color(0.5f, 0.8f, 1f); // Light blue/cyan color
    [SerializeField] private AudioClip timeTickingClip;
    private bool hasPlayedTickingSound = false;
    
    void Awake()
    {
        originalColor = timerText.color;
        StartTimer();
    }

    void Update()
    {
        // Update timer color based on freeze state
        if (isTimerFrozen)
        {
            timerText.color = iceColor;
        }
        else
        {
            timerText.color = originalColor;
        }
        
        if (startTimer)
        {
            if (timeLimit <= 0)
            {
                timeLimit = 0;
                timerText.text = timeLimit.ToString("00.00");
                PlayerFail();
            }
            else
            {
                if (timerRunning && !isTimerFrozen)
                {
                    timeLimit -= Time.deltaTime;
                    timerText.text = timeLimit.ToString("00.00");
                    
                    // Play ticking sound when 3 seconds or less remain
                    if (timeLimit <= 3f && !hasPlayedTickingSound && timeTickingClip != null)
                    {
                        SoundData.PlaySoundFXClip(timeTickingClip, transform.position, 1.0f);
                        hasPlayedTickingSound = true;
                    }
                }
            }
        }

    }

    public void StartTimer()
    {
        startTimer = true;
        timerRunning = true;
        timeisUp = false;
        isTimerFrozen = false;
        hasPlayedTickingSound = false;
        timeLimit = firstStageData.GiveTimeLimit();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
    public void Restart()
    {
        startTimer = true;
        timeLimit = firstStageData.GiveTimeLimit();
        timeisUp = false;
    }
    
    public void PlayerFail()
    {
        Debug.Log("TimerScript: Time is up! Player has failed the stage.");
        startTimer = false;
        timeisUp = true;
        gameManager.FailedtoComplete();
    }
}
