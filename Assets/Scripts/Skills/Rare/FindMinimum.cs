using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMinimum : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManagerScript;
    public bool isActive;
    void Awake()
    {
        isActive = true;
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
        ActivateSkill(gameManagerScript.spawnedCards);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Detect '1' key press
        {
            ActivateSkill(gameManagerScript.spawnedCards);
        }
    }
    
    public void ActivateSkill(List<Card> cards) //FindSmallestCard
    {
        if (isActive == false) return;
        
        if (gameManagerScript.isSwapping) // ignore clicks while swapping
        {
            Debug.Log("Swap in progress - input ignored");
            return;
        }
        
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("Card list is empty or null.");
            return;
        }
        
        int minIndex = 0;
        int minValue = cards[0].Value;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i] == null) continue; // Skip null cardsx
            if (cards[i].Value < minValue)
            {
                minValue = cards[i].Value;
                minIndex = i;
            }
        }

        if (minIndex != 0)
        {
            gameManagerScript.SwapCard(minIndex, 0);
        }

         isActive = false;
    }
}