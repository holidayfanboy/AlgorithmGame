using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSwap : MonoBehaviour
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
            Debug.Log("RandomSwap Skill Activated");
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
        
        if (cards == null || cards.Count < 2)
        {
            Debug.LogWarning("Card list is empty, null, or has less than 2 cards.");
            yield break;
        }
        
        // Pick two random indices
        int firstIndex = Random.Range(0, cards.Count);
        int secondIndex = Random.Range(0, cards.Count);
        
        // Ensure the two indices are different
        while (secondIndex == firstIndex)
        {
            secondIndex = Random.Range(0, cards.Count);
        }
        
        Debug.Log($"RandomSwap: Swapping cards at indices {firstIndex} (value: {cards[firstIndex].Value}) and {secondIndex} (value: {cards[secondIndex].Value})");
        gameManagerScript.SkillSwapCard(firstIndex, secondIndex);
        // Perform the swap - directly yield the coroutine
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);
    }
}
