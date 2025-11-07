using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int PlayerHandRange;
    public int SkillHand;
    public int goldAmount;
    public int playerHealth;
    public List<GameObject> ownedSkills = new List<GameObject>();

    private void OnEnable()
    {
        // Initialize default values when the asset is created
        PlayerHandRange = 1;
        playerHealth = 3;
        goldAmount = 100;
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
    // Add setters to modify values at runtime if needed
    public void SetPlayerHandRange(int range)
    {
        PlayerHandRange = range;
    }

    public void SetSkillHand(int skill)
    {
        SkillHand = skill;
    }

    // Get all owned skills information
    public List<GameObject> GetOwnedSkills()
    {
        return ownedSkills;
    }

    // Get count of owned skills
    public int GetOwnedSkillsCount()
    {
        return ownedSkills.Count;
    }

    // Add a skill to owned skills
    public void AddSkill(GameObject skill)
    {
        if (skill != null && !ownedSkills.Contains(skill))
        {
            ownedSkills.Add(skill);
        }
    }

    // Remove a skill from owned skills
    public bool RemoveSkill(GameObject skill)
    {
        return ownedSkills.Remove(skill);
    }

    // Check if player owns a specific skill
    public bool HasSkill(GameObject skill)
    {
        return ownedSkills.Contains(skill);
    }

    // Clear all owned skills
    public void ClearOwnedSkills()
    {
        ownedSkills.Clear();
    }
}
