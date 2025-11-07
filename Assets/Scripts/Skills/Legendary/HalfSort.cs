using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfSort : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManagerScript;
    public bool isActive;
    
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }
    
    void Update()
    {
        if (isActive)
        {
            Debug.Log("SortLeftHalf Skill Activated");
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
        
        if (cards == null || cards.Count < 2)
        {
            Debug.LogWarning("Card list is empty, null, or has less than 2 cards.");
            yield break;
        }

        // Find the middle index
        int middleIndex = cards.Count / 2;
        Debug.Log($"SortLeftHalf: Middle index is {middleIndex}. Sorting left side (indices 0 to {middleIndex - 1})");
        Color darkGreen = new Color(0f, 0.5f, 0f); 
        // If there's nothing on the left side, exit
        if (middleIndex == 0)
        {
            Debug.Log("SortLeftHalf: No cards on the left side to sort.");
            yield break;
        }

        // Change middle card to yellow
        cards[middleIndex].ChangeColor(Color.yellow);
        
        // Change all left side cards to green
        for (int i = 0; i < middleIndex; i++)
        {
            cards[i].ChangeColor(darkGreen);
        }

        // Perform insertion sort on the left side (indices 0 to middleIndex - 1)
        for (int i = 1; i < middleIndex; i++)
        {
            int currentValue = cards[i].Value;
            int currentIndex = i;
            
            Debug.Log($"SortLeftHalf: Processing card at index {i} with value {currentValue}");
            
            // Compare with cards to the left and swap if needed
            for (int j = i - 1; j >= 0; j--)
            {
                if (cards[currentIndex].Value < cards[j].Value)
                {
                    Debug.Log($"SortLeftHalf: Swapping index {currentIndex} (value: {cards[currentIndex].Value}) with index {j} (value: {cards[j].Value})");
                    
                    gameManagerScript.SkillSwapCard(currentIndex, j);
                yield return new WaitUntil(() => !gameManagerScript.isSwapping);
                    
                    currentIndex = j;
                }
                else
                {
                    break; // Found correct position
                }
            }
        }
        
        // Change all cards back to red after sorting is complete
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].ChangeColor(Color.red);
        }
        
        Debug.Log("SortLeftHalf: Completed sorting left side.");
    }
}
