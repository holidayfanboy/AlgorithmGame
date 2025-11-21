using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    
    [SerializeField] private PlayerData playerData;
    [SerializeField] private FirstStageData stageData;
    
    // Cached values that persist across scene loads
    private int cachedPlayerHandRange;
    private int cachedGoldAmount;
    private int cachedPlayerHealth;
    private int cachedCurrentStage;
    private bool isFirstLoad = true;
    
    private void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameDataManager: Singleton instance created and set to DontDestroyOnLoad");
            
            // Initialize on first load
            if (isFirstLoad)
            {
                InitializeGameData();
                isFirstLoad = false;
            }
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("GameDataManager: Duplicate instance destroyed");
        }
    }
    
    private void InitializeGameData()
    {
        Debug.Log("GameDataManager: Initializing game data for the first time");
        
        if (playerData != null)
        {
            playerData.SetPlayerHandRange(1);
            playerData.IncreasePlayerHealth(-playerData.getPlayerHealth() + 3); // Set to 3
            playerData.IncreaseGoldAmount(-playerData.getGoldAmount() + 100); // Set to 100
            
            if (playerData.ownedSkills != null)
            {
                playerData.ownedSkills.Clear();
            }
            
            CachePlayerData();
        }
        
        if (stageData != null)
        {
            stageData.currentStage = 1;
            stageData.currentRound = 1;
            CacheStageData();
        }
    }
    
    // Call this when entering a new scene to restore data
    public void RestoreGameData()
    {
        if (playerData != null)
        {
            playerData.SetPlayerHandRange(cachedPlayerHandRange);
            playerData.IncreaseGoldAmount(-playerData.getGoldAmount() + cachedGoldAmount);
            playerData.IncreasePlayerHealth(-playerData.getPlayerHealth() + cachedPlayerHealth);
        }
        
        if (stageData != null)
        {
            stageData.currentStage = cachedCurrentStage;
        }
        
        Debug.Log($"GameDataManager: Restored data - Gold: {cachedGoldAmount}, Health: {cachedPlayerHealth}, Stage: {cachedCurrentStage}");
    }
    
    // Call this before leaving a scene to save data
    public void CachePlayerData()
    {
        if (playerData != null)
        {
            cachedPlayerHandRange = playerData.getPlayerHand();
            cachedGoldAmount = playerData.getGoldAmount();
            cachedPlayerHealth = playerData.getPlayerHealth();
            Debug.Log($"GameDataManager: Cached player data - Gold: {cachedGoldAmount}, Health: {cachedPlayerHealth}");
        }
    }
    
    public void CacheStageData()
    {
        if (stageData != null)
        {
            cachedCurrentStage = stageData.currentStage;
            Debug.Log($"GameDataManager: Cached stage data - Stage: {cachedCurrentStage}");
        }
    }
    
    // Call this to reset everything
    public void ResetAllData()
    {
        isFirstLoad = true;
        InitializeGameData();
    }
}
