using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DescriptionData", menuName = "ScriptableObjects/DescriptionData")]
public class DescriptionData : ScriptableObject
{
    [Header("Skill Descriptions by Rarity")]
    [Tooltip("Descriptions for Rare skills - index matches skill order")]
    [TextArea]
    public List<string> rareDescriptions = new List<string>();

    [Tooltip("Descriptions for Epic skills - index matches skill order")]
    [TextArea]
    public List<string> epicDescriptions = new List<string>();
    
    [Tooltip("Descriptions for Legendary skills - index matches skill order")]
    [TextArea]
    public List<string> legendaryDescriptions = new List<string>();

    // Get description by rarity and index
    public string GetDescription(SellingSkill.Rarity rarity, int index)
    {
        List<string> selectedList = null;

        switch (rarity)
        {
            case SellingSkill.Rarity.Rare:
                selectedList = rareDescriptions;
                break;
            case SellingSkill.Rarity.Epic:
                selectedList = epicDescriptions;
                break;
            case SellingSkill.Rarity.Legendary:
                selectedList = legendaryDescriptions;
                break;
        }

        if (selectedList == null || index < 0 || index >= selectedList.Count)
        {
            Debug.LogWarning($"DescriptionData: Invalid rarity '{rarity}' or index '{index}'");
            return "No description available";
        }

        return selectedList[index];
    }

    // Get all descriptions for a specific rarity
    public List<string> GetDescriptionsByRarity(SellingSkill.Rarity rarity)
    {
        switch (rarity)
        {
            case SellingSkill.Rarity.Rare:
                return rareDescriptions;
            case SellingSkill.Rarity.Epic:
                return epicDescriptions;
            case SellingSkill.Rarity.Legendary:
                return legendaryDescriptions;
            default:
                return new List<string>();
        }
    }
}
