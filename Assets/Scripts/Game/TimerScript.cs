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
    void Awake()
    {
        StartTimer();
    }

    void Update()
    {
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
                if (timerRunning)
                {
                    timeLimit -= Time.deltaTime;
                    timerText.text = timeLimit.ToString("00.00");
                }
            }
        }

    }

    public void StartTimer()
    {
        startTimer = true;
        timerRunning = true;
        timeisUp = false;
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
