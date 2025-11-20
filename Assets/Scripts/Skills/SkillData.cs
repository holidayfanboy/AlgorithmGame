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
    public int lifeCost;
    public int rareSellPrice = 50;
    public int epicSellPrice = 100;
    public int legendarySellPrice = 250;
    [SerializeField] private GameObject sellingSkillPrefab;
    public List<GameObject> RareSkills = new List<GameObject>();
    public List<GameObject> EpicSkills = new List<GameObject>();
    public List<GameObject> LegendarySkills = new List<GameObject>();
    
    private static bool hasInitialized = false;
    
    private void OnEnable()
    {
        // Only initialize once when the game first starts
        if (!hasInitialized)
        {
            InitializeDefaultValues();
            hasInitialized = true;
        }
    }

    private void InitializeDefaultValues()
    {
        // Initialize default prices if needed
        if (rareSellPrice == 0) rareSellPrice = 50;
        if (epicSellPrice == 0) epicSellPrice = 100;
        if (legendarySellPrice == 0) legendarySellPrice = 250;
        
        Debug.Log("SkillData: Initialized with default values");
    }

    // Call this manually if you want to reset during gameplay
    public void ResetToDefaults()
    {
        InitializeDefaultValues();
    }

    public GameObject GiveOneSellSkill()
    {
        float randomValue = Random.Range(0f, 100f);

        List<GameObject> selectedList = null;
        SellingSkill.Rarity selectedRarity = SellingSkill.Rarity.Rare;
        int selectedPrice = rareSellPrice;

        if (randomValue < 50f)
        {
            selectedList = RareSkills;
            selectedRarity = SellingSkill.Rarity.Rare;
            selectedPrice = rareSellPrice;
        }
        else if (randomValue < 80f)
        {
            selectedList = EpicSkills;
            selectedRarity = SellingSkill.Rarity.Epic;
            selectedPrice = epicSellPrice;
        }
        else
        {
            selectedList = LegendarySkills;
            selectedRarity = SellingSkill.Rarity.Legendary;
            selectedPrice = legendarySellPrice;
        }

        if (selectedList == null || selectedList.Count == 0)
        {
            Debug.LogWarning($"GiveOneSellSkill: {selectedRarity} list empty");
            return null;
        }

        int randomIndex = Random.Range(0, selectedList.Count);
        GameObject selectedSkill = selectedList[randomIndex];

        if (sellingSkillPrefab == null)
        {
            Debug.LogError("GiveOneSellSkill: sellingSkillPrefab missing");
            return null;
        }

        GameObject sellingSkillInstance = Instantiate(sellingSkillPrefab);
        var sellingSkillComponent = sellingSkillInstance.GetComponent<SellingSkill>();

        if (sellingSkillComponent == null)
        {
            Debug.LogError("GiveOneSellSkill: prefab lacks SellingSkill component");
            Destroy(sellingSkillInstance);
            return null;
        }

        sellingSkillComponent.SellPrice = selectedPrice;
        sellingSkillComponent.skillRarity = selectedRarity;
        sellingSkillComponent.skillObject = selectedSkill;
        sellingSkillComponent.skillIndex = randomIndex;

        Debug.Log($"GiveOneSellSkill: {selectedRarity} -> {selectedSkill.name} (idx {randomIndex}) price {selectedPrice}");
        return sellingSkillInstance;
    }

    public void IncreaseLifeCost()
    {
        lifeCost *= 2;
        Debug.Log($"SkillData: Increased lifeCost to {lifeCost}");
    }
}

abstract class SkillEffect
{
    public abstract void Execute(List<Card> target);
}