using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalLayoutNonUI : MonoBehaviour
{
    [SerializeField] private float startX = -5.5f;
    [SerializeField] private float endX = 5.8f;
    [SerializeField] private float spacing = 0f; // Optional: additional spacing between objects
    [SerializeField] private GameObject enemyObject;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<GameObject> spawnedEnemies = new List<GameObject>();
    [SerializeField] private AnimationCurve swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private GameObject player;

    

    public IReadOnlyList<GameObject> SpawnedEnemies => spawnedEnemies;
    void Start()
    {
        ArrangeChildren();
    }

    
    public void ArrangeChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        float totalWidth = endX - startX;
        float spacing = childCount > 1 ? totalWidth / (childCount - 1) : 0;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float xPos = startX + (spacing * i);
            child.localPosition = new Vector3(xPos, child.localPosition.y, child.localPosition.z);
        }

        Debug.Log($"HorizontalLayoutNonUI: Arranged {childCount} children from {startX} to {endX}");
    }

    // Call this if children are added/removed at runtime
    public void Refresh()
    {
        ArrangeChildren();
    }

    public void SpawnEnemy(int count)
    {
        // Remove all existing enemies from hierarchy (but don't destroy)
        ClearEnemies();

        // Update layout bounds relative to player before spawning
        UpdateBoundsFromPlayer();

        if (count <= 0)
        {
            Debug.LogWarning("SpawnEnemy called with non-positive count.");
            return;
        }

        if (enemyObject == null)
        {
            Debug.LogWarning("SpawnEnemy aborted: enemyObject prefab not assigned.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject instance = Instantiate(enemyObject, transform);
            // Local position will be overridden by ArrangeChildren anyway; keep Y/Z from prefab.
            instance.transform.localPosition = new Vector3(0f, instance.transform.localPosition.y, instance.transform.localPosition.z);
            spawnedEnemies.Add(instance);
        }

        ArrangeChildren();
    }

    // Remove enemies from parent (but don't destroy them)
    private void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                enemy.transform.SetParent(null);
            }
        }
        spawnedEnemies.Clear();
    }

    // Keep the layout span width but reposition it so startX is at player's X + 2 (in local space)
    private void UpdateBoundsFromPlayer()
    {
        if (player == null) return;

        float width = endX - startX;

        Vector3 worldTarget = new Vector3(player.transform.position.x + 2f, transform.position.y, transform.position.z);
        float newLocalStartX = transform.InverseTransformPoint(worldTarget).x;

        startX = newLocalStartX;
        endX = startX + width;
    }

    // Rebuild list from current children if manual edits occurred
    public void ResyncSpawnedEnemies()
    {
        spawnedEnemies.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnedEnemies.Add(transform.GetChild(i).gameObject);
        }
    }

    public void EnemySwapAnimated(GameObject first, GameObject second, float duration = 0.5f)
    {
        if (first == null || second == null)
        {
            Debug.LogWarning("EnemySwapAnimated: one or both GameObjects are null.");
            return;
        }
        if (first == second)
        {
            Debug.LogWarning("EnemySwapAnimated: both references point to the same GameObject.");
            return;
        }
        StartCoroutine(EnemySwapCoroutine(first, second, duration));
    }

    public IEnumerator EnemySwapCoroutine(GameObject first, GameObject second, float duration)
    {
        Vector3 startA = first.transform.localPosition;
        Vector3 startB = second.transform.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(duration, 0.0001f);
            float eased = swapCurve.Evaluate(Mathf.Clamp01(t));
            first.transform.localPosition = Vector3.Lerp(startA, startB, eased);
            second.transform.localPosition = Vector3.Lerp(startB, startA, eased);
            yield return null;
        }
        // Ensure final positions exact
        first.transform.localPosition = startB;
        second.transform.localPosition = startA;
    }

    // Overload: swap by indices in spawnedEnemies using animated swap
    public void EnemySwap(int first, int second)
    {
        if (first == second)
        {
            Debug.LogWarning("EnemySwap(int,int): indices are the same; no swap performed.");
            return;
        }
        if (first < 0 || second < 0 || first >= spawnedEnemies.Count || second >= spawnedEnemies.Count)
        {
            Debug.LogWarning($"EnemySwap(int,int): invalid indices first={first} second={second} count={spawnedEnemies.Count}");
            return;
        }
        // Swap list entries to keep logical order in sync with the intended visual swap
        GameObject temp = spawnedEnemies[first];
        spawnedEnemies[first] = spawnedEnemies[second];
        spawnedEnemies[second] = temp;

        // Animate the visual swap
        EnemySwapAnimated(spawnedEnemies[first], spawnedEnemies[second], 0.5f);
    }


    public void ReorderSpawnedEnemiesLogical()
    {
        spawnedEnemies.RemoveAll(go => go == null);

        spawnedEnemies.Sort((a, b) => a.transform.localPosition.x.CompareTo(b.transform.localPosition.x));

        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i] != null)
            {
                spawnedEnemies[i].transform.SetSiblingIndex(i);
            }
        }

        ArrangeChildren();
    }
    

}