using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOneCard : MonoBehaviour
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
            Debug.Log("DeleteRandomCard Skill Activated");
            ActivateSkill(gameManagerScript.spawnedCards);
            isActive = false;
        }
    }
    
    private void ActivateSkill(List<Card> cards)
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("DeleteRandomCard: Card list is empty or null.");
            return;
        }
        
        // Pick a random index from the card list
        int randomIndex = Random.Range(0, cards.Count);
        
        Debug.Log($"DeleteRandomCard: Deleting card at random index {randomIndex} with value {cards[randomIndex].Value}");
        
        // Delete the card at the random index
        gameManagerScript.DeleteCard(randomIndex);
        
        Debug.Log("DeleteRandomCard: Card deleted successfully.");
    }
}
