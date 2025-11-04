using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMinimum : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private GameManager gameManagerScript;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }
    void Update()
    {

    }
    
    public void ActivateSkill(List<Card> cards) //FindSmallestCard
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("Card list is empty or null.");
            return;
        }
        
        int minIndex = 0;
        int minValue = cards[0].Value;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i] == null) continue; // Skip null cards
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
    }
}
