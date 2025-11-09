using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMedian : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManagerScript;
    public bool isActive;
    public int sellPrice;
    
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }
    
    void Update()
    {
        if (isActive)
        {
            Debug.Log("FindMedian Skill Activated");
            StartCoroutine(ActivateSkillCoroutine(gameManagerScript.spawnedCards));
            isActive = false; 
        }
    }
    
    private IEnumerator ActivateSkillCoroutine(List<Card> cards)
    {
        if (gameManagerScript.isSwapping)
        {
            Debug.Log("Swap in progress - input ignored");
            yield break;
        }
        
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("Card list is empty or null.");
            yield break;
        }

        // Create a sorted copy of the values to find the median value
        List<int> sortedValues = cards.Select(c => c.Value).OrderBy(v => v).ToList();
        
        int medianValue;
        if (sortedValues.Count % 2 == 0)
        {
            // Even number of cards - take the lower middle value
            medianValue = sortedValues[sortedValues.Count / 2 - 1];
        }
        else
        {
            // Odd number of cards - take the exact middle value
            medianValue = sortedValues[sortedValues.Count / 2];
        }
        
        Debug.Log($"FindMedian: Median value is {medianValue}");

        // Find the card with the median value in the original list
        int medianIndex = -1;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].Value == medianValue)
            {
                medianIndex = i;
                break;
            }
        }

        if (medianIndex == -1)
        {
            Debug.LogWarning("FindMedian: Could not find card with median value.");
            yield break;
        }

        // Calculate the middle position
        int middleIndex = cards.Count / 2;

        // If the median card is already in the middle, do nothing
        if (medianIndex == middleIndex)
        {
            Debug.Log($"FindMedian: Median card (value: {medianValue}) is already at middle position {middleIndex}");
            yield break;
        }

        // Swap the median card to the middle position
        Debug.Log($"FindMedian: Swapping card at index {medianIndex} (value: {medianValue}) to middle position {middleIndex}");
        gameManagerScript.SkillSwapCard(medianIndex, middleIndex);
        // Perform the swap - directly yield the coroutine
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);
    }
}
