using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "FirstStageData", menuName = "ScriptableObjects/StageData")]
public class FirstStageData : ScriptableObject
{
    public enum State
    {
        game, shop, rest, boss
    }
    public State state;
    public int currentStage;
    public int currentRound;
    public int Stage;

    public int MaxStage;

    public int stageEnd;
    public List<int> cardSize = new List<int>();
    public bool isBoss;
    public List<float> timeLimits = new List<float>();
    private static bool hasInitialized = false;

    public bool stageSuccess;

    public int GiveMaxStageSize()
    {
        return cardSize.Count;
    }
    
    public void SetStageSuccess()
    {
        stageSuccess = true;
    }

    public void SetStageFail()
    {
        stageSuccess = false;
    }

    public void IncreaseStage()
    {
        currentStage++;
    }

    public float GiveTimeLimit()
    {
        if (currentStage - 1 < timeLimits.Count)
        {
            return timeLimits[currentStage - 1];
        }
        else
        {
            Debug.LogWarning("StageData: currentStage exceeds timeLimits list. Returning default 10s.");
            return 10f; // Default time limit if out of range
        }
    }
    public int GiveCardSize()
    {
        if (currentStage - 1 < cardSize.Count)
        {
            return cardSize[currentStage - 1];
        }
        else
            Debug.LogWarning("StageData: currentStage exceeds cardSize list. Returning default max value 10.");
        return 10; // Default max value if out of range
    }
    private void OnEnable()
    {
        // Only initialize once when the game first starts
        if (!hasInitialized)
        {
            InitializeDefaultValues();
            hasInitialized = true;
        }
    }

    private void InitializeDefaultValues()
    {
        //Initialize default values when the asset is created
        state = State.game;
        currentStage = 1;
        // state = State.boss;
        // isBoss = true;
        // currentStage = 6;
        currentRound = 1;
        MaxStage = 3;
        stageEnd = 6;
        Debug.Log("StageData: Initialized with default values");
    }

    // Call this manually if you want to reset during gameplay
    public void ResetToDefaults()
    {
        InitializeDefaultValues();
    }

    public void SetStateToGame()
    {
        state = State.game;
        ChangeScene();
    }

    public void SetStateToShop()
    {
        state = State.shop;
        ChangeScene();
    }

    public void SetStateToBoss()
    {
        state = State.boss;
        ChangeScene();
    }

    public void SetStateToRest()
    {
        state = State.rest;
        ChangeScene();
    }

    public void ChangeState(State newState)
    {
        state = newState;
        ChangeScene();
    }

    private void ChangeScene()
    {
        // Check if currentStage is 6 and state is rest, then go to boss scene
        if (currentStage == 6 && state == State.rest)
        {
            state = State.boss;
            isBoss = true;
            Debug.Log("StageData: Stage 6 reached from Rest - Setting to Boss state");
        }
        
        switch (state)
        {
            case State.game:
                Debug.Log("StageData: Loading GameScene");
                SceneManager.LoadScene("GameScene");
                break;
            case State.shop:
                Debug.Log("StageData: Loading ShopScene");
                SceneManager.LoadScene("ShopScene");
                break;
            case State.rest:
                Debug.Log("StageData: Loading RestScene");
                SceneManager.LoadScene("RestScene");
                break;
            case State.boss:
                Debug.Log("StageData: Loading BossScene");
                SceneManager.LoadScene("FinalBossScene");
                break;
            default:
                Debug.LogWarning("StageData: Unknown state");
                break;
        }
    }
}
