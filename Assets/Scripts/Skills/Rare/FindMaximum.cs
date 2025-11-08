using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMaximum : MonoBehaviour
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
            Debug.Log("FindMaximum Skill Activated");
            StartCoroutine(ActivateSkillCoroutine(gameManagerScript.spawnedCards));
            isActive = false; // prevent multiple activations
        }
    }

    private IEnumerator ActivateSkillCoroutine(List<Card> cards)
    {
        if (gameManagerScript.isSwapping) // ignore if already swapping
        {
            Debug.Log("Swap in progress - input ignored");
            yield break;
        }
        
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("Card list is empty or null.");
            yield break;
        }
        
        int maxIndex = 0;
        int maxValue = cards[0].Value;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;
            if (cards[i].Value > maxValue)
            {
                maxValue = cards[i].Value;
                maxIndex = i;
            }
        }

        int lastIndex = cards.Count - 1;

        // If the maximum card is not already at the last position, swap it
        if (maxIndex != lastIndex)
        {
            gameManagerScript.SkillSwapCard(lastIndex, maxIndex); // Changed from SwapCard
            Debug.Log($"Maximum value card ({maxValue}) moved to position {lastIndex}");
            
            // Wait for the swap animation to complete
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);
        }
        else
        {
            Debug.Log($"Maximum value card ({maxValue}) is already at position {lastIndex}");
        }
    }
}
