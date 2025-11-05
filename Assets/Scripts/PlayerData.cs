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
}
