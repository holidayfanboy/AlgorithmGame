// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class DataManager : MonoBehaviour
// {
//     [SerializeField] private PlayerData playerData;
 

//     void Awake()
//     {
//         if (playerData == null)
//         {
//             Debug.LogError("PlayerData not assigned to DataManager!");
//             return;
//         }

//         // Initialize UI with current values from PlayerData
//         RefreshUI();
//     }

//     void Update()
//     {
        
//     }

//     // Updates both the PlayerData and UI
//     public void UpdateGold(int amount)
//     {
//         if (playerData == null) return;
        
//         playerData.IncreaseGoldAmount(amount);
//         goldText.text = playerData.getGoldAmount().ToString();
//     }
    
//     public void UpdateHealth(int amount)
//     {
//         if (playerData == null) return;
        
//         playerData.IncreasePlayerHealth(amount);
//         playerHealthText.text = playerData.getPlayerHealth().ToString();
//     }

//     // Refresh UI displays with current PlayerData values
//     private void RefreshUI()
//     {
//         if (goldText != null)
//             goldText.text = playerData.getGoldAmount().ToString();
        
//         if (playerHealthText != null)
//             playerHealthText.text = playerData.getPlayerHealth().ToString();
//     }
// }
