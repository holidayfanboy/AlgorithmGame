using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SellLife : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SkillData skillData;
    [SerializeField] private PlayerData playerData;
    private string description;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject skillName;
    [SerializeField] private ShopPlayer shopPlayer; 

    void Awake()
    {
        if (shopPlayer == null)
        {
            shopPlayer = FindObjectOfType<ShopPlayer>();
        }
        if (skillName != null)
            skillName.SetActive(false);
        description = $"수명 +1 을 증가 시켜 줍니다";
        descriptionText.text = description + $"\n가격: {skillData.lifeCost})";
    }

    public void SellLifeToPlayer()
    {
        if (playerData == null)
        {
            Debug.LogWarning("SellLifeToPlayer: PlayerData missing.");
            return;
        }

        if (playerData.goldAmount < skillData.lifeCost)
        {
            Debug.Log("SellLifeToPlayer: Not enough gold to sell life.");
            return;
        }

        shopPlayer.UpdateGold(-skillData.lifeCost);
        shopPlayer.UpdateHealth(1);
        skillData.IncreaseLifeCost();

        Debug.Log($"SellLifeToPlayer: Sold life for {skillData.lifeCost} gold. New health: {playerData.playerHealth}, Remaining gold: {playerData.goldAmount}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillName != null)
            skillName.SetActive(true);
    }

    // Called when mouse exits the SellingSkill
    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillName != null)
            skillName.SetActive(false);
    }

}
