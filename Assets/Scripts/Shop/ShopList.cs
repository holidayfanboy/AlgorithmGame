using System.Collections.Generic;
using UnityEngine;

public class ShopList : MonoBehaviour
{
    [SerializeField] private SkillData skillData;
    [SerializeField] private int offerCount = 3;
    [SerializeField] private Transform contentRoot; // optional; if null uses own transform
    [SerializeField] private GameObject sellLifePrefab;
    private readonly List<GameObject> currentOffers = new List<GameObject>();
    private readonly List<GameObject> usedSkillObjects = new List<GameObject>();

    void Start()
    {
        PopulateShop();
    }

    public void PopulateShop()
    {
        ClearCurrent();
        usedSkillObjects.Clear();

        if (skillData == null)
        {
            Debug.LogWarning("ShopList: SkillData not assigned");
            return;
        }

        Transform parent = contentRoot != null ? contentRoot : transform;

        for (int i = 0; i < offerCount; i++)
        {
            GameObject offer = GetUniqueSkillOffer();
            if (offer == null)
            {
                Debug.LogWarning($"ShopList: Could not generate unique offer {i + 1}");
                continue;
            }

            offer.transform.SetParent(parent, false);
            currentOffers.Add(offer);
        }

        Debug.Log($"ShopList: Spawned {currentOffers.Count} offers");

        if (sellLifePrefab != null)
        {
            GameObject sellLifeInstance = Instantiate(sellLifePrefab, parent, false);
            currentOffers.Add(sellLifeInstance);
            Debug.Log("ShopList: Added sellLifePrefab");
        }
        else
        {
            Debug.LogWarning("ShopList: sellLifePrefab not assigned");
        }
    }

    private GameObject GetUniqueSkillOffer()
    {
        const int maxAttempts = 20; // Prevent infinite loop
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            GameObject offer = skillData.GiveOneSellSkill();
            if (offer == null) continue;

            SellingSkill sellingSkill = offer.GetComponent<SellingSkill>();
            if (sellingSkill == null || sellingSkill.skillObject == null)
            {
                Destroy(offer);
                continue;
            }

            // Check if this skill is already in the list
            if (!usedSkillObjects.Contains(sellingSkill.skillObject))
            {
                usedSkillObjects.Add(sellingSkill.skillObject);
                return offer;
            }

            // Duplicate found, destroy and try again
            Debug.Log($"ShopList: Duplicate skill {sellingSkill.skillObject.name} found, regenerating...");
            Destroy(offer);
        }

        Debug.LogWarning("ShopList: Failed to generate unique skill after max attempts");
        return null;
    }

    public void RefreshShop()
    {
        PopulateShop();
    }

    private void ClearCurrent()
    {
        for (int i = 0; i < currentOffers.Count; i++)
        {
            if (currentOffers[i] != null)
                Destroy(currentOffers[i]);
        }
        currentOffers.Clear();
    }
}