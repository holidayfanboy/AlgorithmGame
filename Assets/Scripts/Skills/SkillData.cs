using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public float cooldown;
    public int power;
    public int price;
    public int rareSellPrice = 50;
    public int epicSellPrice = 100;
    public int legendarySellPrice = 250;
    [SerializeField] private GameObject sellingSkillPrefab;
    public List<GameObject> RareSkills = new List<GameObject>();
    public List<GameObject> EpicSkills = new List<GameObject>();
    public List<GameObject> LegendarySkills = new List<GameObject>();

    public GameObject GiveOneSellSkill()
    {
        // Generate a random number between 0 and 100
        float randomValue = Random.Range(0f, 100f);

        List<GameObject> selectedList = null;
        SellingSkill.Rarity selectedRarity = SellingSkill.Rarity.Rare;
        int selectedPrice = rareSellPrice;

        // 70% chance for Rare (0-70)
        if (randomValue < 70f)
        {
            selectedList = RareSkills;
            selectedRarity = SellingSkill.Rarity.Rare;
            selectedPrice = rareSellPrice;
        }
        // 20% chance for Epic (70-90)
        else if (randomValue < 90f)
        {
            selectedList = EpicSkills;
            selectedRarity = SellingSkill.Rarity.Epic;
            selectedPrice = epicSellPrice;
        }
        // 10% chance for Legendary (90-100)
        else
        {
            selectedList = LegendarySkills;
            selectedRarity = SellingSkill.Rarity.Legendary;
            selectedPrice = legendarySellPrice;
        }

        // Check if the selected list is empty
        if (selectedList == null || selectedList.Count == 0)
        {
            Debug.LogWarning($"GiveOneSellSkill: {selectedRarity} skill list is empty!");
            return null;
        }

        // Pick a random skill from the selected list
        int randomIndex = Random.Range(0, selectedList.Count);
        GameObject selectedSkill = selectedList[randomIndex];

        // Instantiate the selling skill prefab
        if (sellingSkillPrefab == null)
        {
            Debug.LogError("GiveOneSellSkill: sellingSkillPrefab is not assigned!");
            return null;
        }

        GameObject sellingSkillInstance = Instantiate(sellingSkillPrefab);
        SellingSkill sellingSkillComponent = sellingSkillInstance.GetComponent<SellingSkill>();

        if (sellingSkillComponent != null)
        {
            // Set the properties on the SellingSkill component
            sellingSkillComponent.SellPrice = selectedPrice;
            sellingSkillComponent.skillRarity = selectedRarity;
            sellingSkillComponent.skillObject = selectedSkill;

            Debug.Log($"GiveOneSellSkill: Created {selectedRarity} skill - {selectedSkill.name} with price {selectedPrice}");
        }
        else
        {
            Debug.LogError("GiveOneSellSkill: sellingSkillPrefab does not have SellingSkill component!");
            Destroy(sellingSkillInstance);
            return null;
        }

        return sellingSkillInstance;
    }
}

abstract class SkillEffect
{
    public abstract void Execute(List<Card> target);
}