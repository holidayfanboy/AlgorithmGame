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

    public void Awake()
    {
        skillIndex = 0;
        skillRarity = Rarity.Epic;
        // Hide skill name at start
        if (skillName != null)
        {
            skillName.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    // Called when mouse enters the SellingSkill
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillName != null)
        {
            skillName.SetActive(true);
            Debug.Log("SellingSkill: skillName enabled");
        }

        UpdateSkillDescription();
    }

    // Called when mouse exits the SellingSkill
    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillName != null)
        {
            skillName.SetActive(false);
            Debug.Log("SellingSkill: skillName disabled");
        }
    }

    private void UpdateSkillDescription()
    {
        if (skillDescription == null)
        {
            return;
        }

        if (descriptionData == null)
        {
            Debug.LogWarning("SellingSkill: DescriptionData is not assigned!");
            skillDescription.text = "No description available\n" + "가격: " + SellPrice.ToString();
            return;
        }

        // Get description from DescriptionData using skillRarity and skillIndex
        string description = descriptionData.GetDescription(skillRarity, skillIndex);
        skillDescription.text = description + "\n" + SellPrice.ToString();
        
        Debug.Log($"SellingSkill: Set description for {skillRarity} skill at index {skillIndex}: '{description}'");
    }
}
