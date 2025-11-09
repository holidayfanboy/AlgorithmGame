using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillLayoutShop : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Transform contentRoot;     // optional parent
    private int firstIndex = -1;
    private int secondIndex = -1;
    private readonly List<GameObject> spawned = new List<GameObject>();

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        Clear();
        if (playerData == null || playerData.ownedSkills == null) return;
        Transform parent = contentRoot ? contentRoot : transform;

        for (int i = 0; i < playerData.ownedSkills.Count; i++)
        {
            GameObject prefab = playerData.ownedSkills[i];
            if (!prefab) continue;

            GameObject instance = Instantiate(prefab, parent, false);
            spawned.Add(instance);

            // Add click helper
            if (!instance.TryGetComponent(out OwnedSkillClickable clickable))
                clickable = instance.AddComponent<OwnedSkillClickable>();
            clickable.Init(this);
        }

        firstIndex = -1;
        secondIndex = -1;
    }

    public void OnSkillClicked(GameObject obj)
    {
        int idx = obj.transform.GetSiblingIndex();

        // If clicking same as first -> deselect
        if (firstIndex == idx && secondIndex == -1)
        {
            firstIndex = -1;
            return;
        }

        // First selection
        if (firstIndex == -1)
        {
            firstIndex = idx;
            return;
        }

        // Prevent selecting same twice
        if (idx == firstIndex) return;

        // Second selection
        secondIndex = idx;

        // Swap
        Swap(firstIndex, secondIndex);

        // Clear selection
        firstIndex = -1;
        secondIndex = -1;
    }

    private void Swap(int a, int b)
    {
        if (a < 0 || b < 0 || a == b) return;

        Transform parent = contentRoot ? contentRoot : transform;

        Transform ta = parent.GetChild(a);
        Transform tb = parent.GetChild(b);

        ta.SetSiblingIndex(b);
        tb.SetSiblingIndex(a);

        // Rebuild ownedSkills order to match new sibling order
        ReassembleOwnedSkills();

        Debug.Log($"SkillLayoutShop: Swapped {a} <-> {b}");
    }

    public void ReassembleOwnedSkills()
    {
        if (playerData == null || playerData.ownedSkills == null) return;

        Transform parent = contentRoot ? contentRoot : transform;
        var newOrder = new List<GameObject>();

        // Go through each child in sibling order
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject spawnedInstance = spawned.Find(g => g != null && g.transform.GetSiblingIndex() == i);
            if (spawnedInstance == null) continue;

            // Try to find the original prefab by matching name (remove "(Clone)" suffix)
            string cleanName = spawnedInstance.name.Replace("(Clone)", "").Trim();
            GameObject originalPrefab = playerData.ownedSkills.Find(p => p != null && p.name == cleanName);

            // Add original prefab if found, otherwise keep the instance reference
            if (originalPrefab != null)
            {
                newOrder.Add(originalPrefab);
            }
            else
            {
                Debug.LogWarning($"ReassembleOwnedSkills: Could not find original prefab for {cleanName}");
                newOrder.Add(spawnedInstance);
            }
        }

        playerData.ownedSkills = newOrder;
        Debug.Log($"ReassembleOwnedSkills: Updated PlayerData.ownedSkills with {newOrder.Count} skills");
    }

    private void Clear()
    {
        foreach (var g in spawned)
            if (g) Destroy(g);
        spawned.Clear();
    }
}

// Helper component to detect clicks
public class OwnedSkillClickable : MonoBehaviour, IPointerClickHandler
{
    private SkillLayoutShop layout;

    public void Init(SkillLayoutShop l) => layout = l;

    public void OnPointerClick(PointerEventData eventData)
    {
        layout?.OnSkillClicked(gameObject);
    }
}
