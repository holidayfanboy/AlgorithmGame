using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int PlayerHandRange;

    [SerializeField] private FirstStageData firstStageData;
    public int SkillHand;
    public int goldAmount;
    public int playerHealth;
    public bool isMove;
    public List<GameObject> ownedSkills = new List<GameObject>();
    public int ownedSkillCount;
    
    private static bool hasInitialized = false; // Changed to static to persist across scene loads
    
    public void ActivateMove()
    {
        isMove = true;
    }

    public void DeactivateMove()
    {
        isMove = false;
    }
    
    private void OnEnable()
    {
        isMove = false;
        
        // Don't auto-initialize if GameDataManager exists
        if (GameDataManager.Instance != null)
        {
            Debug.Log("PlayerData: GameDataManager detected, skipping auto-initialization");
            return;
        }
        
        // Only initialize once when the game first starts (fallback if no GameDataManager)
        if (!hasInitialized)
        {
            InitializeDefaultValues();
            hasInitialized = true;
        }
    }

    private void InitializeDefaultValues()
    {
        Debug.Log("PlayerData: Initializing default values");
        // Initialize default values when the asset is created
        PlayerHandRange = 1;
        playerHealth = 3;
        goldAmount = 100;
        ownedSkillCount = 5;
        
        // Clear owned skills whenever the game restarts
        if (ownedSkills == null)
        {
            ownedSkills = new List<GameObject>();
        }
        else
        {
            ownedSkills.Clear();
        }
        hasInitialized = true;
        Debug.Log("PlayerData: Initialized with default values and cleared owned skills");
    }

    // Call this manually if you want to reset during gameplay
    public void ResetToDefaults()
    {
        InitializeDefaultValues();
    }

    public int getPlayerHand()
    {
        return PlayerHandRange;
    }

    public int getGoldAmount()
    {
        return goldAmount;
    }

    public int getPlayerHealth()
    {
        return playerHealth;
    }

    public void IncreaseGoldAmount(int amount)
    {
        goldAmount += amount;
    }
    
    public void IncreasePlayerHealth(int health)
    {
        playerHealth += health;
    }

    public void SetPlayerHandRange(int range)
    {
        PlayerHandRange = range;
    }

    public void SetSkillHand(int skill)
    {
        SkillHand = skill;
    }

    public List<GameObject> GetOwnedSkills()
    {
        return ownedSkills;
    }

    public int GetOwnedSkillsCount()
    {
        return ownedSkills.Count;
    }

    public void AddSkill(GameObject skill)
    {
        if (skill != null && !ownedSkills.Contains(skill))
        {
            ownedSkills.Add(skill);
        }
    }

    public bool RemoveSkill(GameObject skill)
    {
        return ownedSkills.Remove(skill);
    }

    public bool HasSkill(GameObject skill)
    {
        return ownedSkills.Contains(skill);
    }

    public void ClearOwnedSkills()
    {
        ownedSkills.Clear();
    }
}
