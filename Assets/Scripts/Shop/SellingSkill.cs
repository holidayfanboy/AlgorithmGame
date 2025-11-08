using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SellingSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Rarity // Made public so SkillData can access it
    { 
        Rare,
        Epic,
        Legendary
    }

    public PlayerData playerData;
    public DescriptionData descriptionData;
    public int SellPrice;
    public Rarity skillRarity;
    public GameObject skillObject;
    public TMP_Text skillDescription;
    public GameObject skillName;
    public int skillIndex;

    [SerializeField] private ShopPlayer shopPlayer; 
    [SerializeField] private SkillLayoutShop skillLayoutShop;
    public void Awake()
    {
        if (shopPlayer == null)
        {
            shopPlayer = FindObjectOfType<ShopPlayer>();
        }

        if (skillName != null)
            skillName.SetActive(false);
        
        if (skillLayoutShop == null)
        {
            skillLayoutShop = FindObjectOfType<SkillLayoutShop>();
        }
    }

    void Update()
    {
        
    }

    // Called when mouse enters the SellingSkill
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillName != null)
            skillName.SetActive(true);

        UpdateSkillDescription();
    }

    // Called when mouse exits the SellingSkill
    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillName != null)
            skillName.SetActive(false);
    }

    private void UpdateSkillDescription()
    {
        if (skillDescription == null)
            return;

        if (descriptionData == null)
        {
            skillDescription.text = "No description available\n가격: " + SellPrice;
            return;
        }

        string description = descriptionData.GetDescription(skillRarity, skillIndex);
        skillDescription.text = description + "\n가격: " + SellPrice;
    }

    public void SellingSkilltoPlayer()
    {
        if (playerData == null)
        {
            Debug.LogWarning("SellingSkilltoPlayer: PlayerData missing.");
            return;
        }
        if (skillObject == null)
        {
            Debug.LogWarning("SellingSkilltoPlayer: skillObject missing.");
            return;
        }

        // Ensure list exists
        if (playerData.ownedSkills == null)
        {
            playerData.ownedSkills = new List<GameObject>();
        }

        // Capacity check BEFORE spending gold
        if (playerData.ownedSkills.Count >= playerData.ownedSkillCount)
        {
            Debug.Log("SellingSkilltoPlayer: Cannot buy. Owned skills are at capacity.");
            return;
        }

        // Check gold
        int currentGold = playerData.goldAmount;
        if (currentGold < SellPrice)
        {
            Debug.Log("SellingSkilltoPlayer: Not enough gold.");
            return;
        }

        // Use ShopPlayer to modify gold
        if (shopPlayer != null)
        {
            shopPlayer.UpdateGold(-SellPrice);
        }
        else
        {
            Debug.LogWarning("SellingSkilltoPlayer: ShopPlayer missing.");
            return;
        }

        playerData.ownedSkills.Add(skillObject);
        Debug.Log($"SellingSkilltoPlayer: Bought {skillObject.name} for {SellPrice}. Total owned: {playerData.ownedSkills.Count}");
        skillLayoutShop.Refresh();
        Destroy(gameObject);
    }
}

