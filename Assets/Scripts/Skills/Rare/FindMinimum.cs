using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMinimum : MonoBehaviour
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
            Debug.Log("FindMinimum Skill Activated");
            StartCoroutine(ActivateSkillCoroutine(gameManagerScript.spawnedCards));
            isActive = false; 
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
        
        int minIndex = 0;
        int minValue = cards[0].Value;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;
            if (cards[i].Value < minValue)
            {
                minValue = cards[i].Value;
                minIndex = i;
            }
        }

        if (minIndex != 0)
        {
            gameManagerScript.SkillSwapCard(minIndex, 0); // Changed from SwapCard
            
            // Wait for the swap animation to complete
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);
        }
    }
}