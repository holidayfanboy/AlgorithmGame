using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataManager : MonoBehaviour
{
    public int goldAmount;
    public int timePlayed;
    public int playerHealth;
    public TMP_Text goldText;
    public TMP_Text playerHealthText;

    void Awake()
    {
        playerHealth = 3;
        goldAmount = 100;
        UpdateGold(0); 
        UpdateHealth(0);
    }

    void Update()
    {
    }

    // Call this method whenever you change the goldAmount
    public void UpdateGold(int amount)
    {
        goldAmount += amount;
        goldText.text = goldAmount.ToString(); 
    }
    
    public void UpdateHealth(int amount)
    {
        playerHealth += amount;
        playerHealthText.text = playerHealth.ToString();
    }
}
