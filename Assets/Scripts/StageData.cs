using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public enum State
    {
        game, shop, rest
    }
    public State state;
    public int currentStage;
    public int currentRound;
    public int Stage;

    public int MaxStage;

    public int CurrentRound;

    public class RoundInfo
    {
        public int stageEnd;
        public int cardSize;
        public bool isBoss;
    };

    public List<RoundInfo> rounds = new List<RoundInfo>();
    
    private void OnEnable()
    {
        // Initialize default values when the asset is created
        state = State.game;
        currentStage = 1;
        currentRound = 1;
        MaxStage = 3;
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
            default:
                Debug.LogWarning("StageData: Unknown state");
                break;
        }
    }
}
