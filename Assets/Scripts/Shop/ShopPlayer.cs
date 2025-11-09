using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ShopPlayer : MonoBehaviour
{
    public TMP_Text goldText;
    public TMP_Text playerHealthText;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private FirstStageData stage;

    // Start is called before the first frame update
    void Awake()
    {
        UpdateGold(0);
        UpdateHealth(0);
    }

    // Update is called once per frame
    public void UpdateGold(int amount)
    {
        playerData.IncreaseGoldAmount(amount);
        goldText.text = playerData.getGoldAmount().ToString();
    }

    public void UpdateHealth(int amount)
    {
        playerData.IncreasePlayerHealth(amount);
        playerHealthText.text = playerData.getPlayerHealth().ToString();
    }
}
