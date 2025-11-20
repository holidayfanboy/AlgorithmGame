using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillOneCard : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManagerScript;
    [SerializeField] private Image logo; // UI Image for logo
    [SerializeField] private float fadeOutDuration = 1f; // Duration for fade out effect
    public bool isActive;
    public int sellPrice;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
        
        // Set logo to fully transparent initially
        if (logo != null)
        {
            Color color = logo.color;
            color.a = 0f; // Start transparent
            logo.color = color;
        }
    }
    
    void Update()
    {
        if (isActive)
        {
            Debug.Log("DeleteRandomCard Skill Activated");
            
            // Set logo to fully opaque when activated
            if (logo != null)
            {
                Color color = logo.color;
                color.a = 1f; // Set to 255/255 = 1.0
                logo.color = color;
            }
            
            StartCoroutine(ActivateSkillCoroutine(gameManagerScript.spawnedCards));
            isActive = false;
        }
    }
    
    private IEnumerator ActivateSkillCoroutine(List<Card> cards)
    {
        // Start fading out the logo
        
        
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("DeleteRandomCard: Card list is empty or null.");
            yield break;
        }
        
        // Pick a random index from the card list
        int randomIndex = Random.Range(0, cards.Count);
        
        Debug.Log($"DeleteRandomCard: Deleting card at random index {randomIndex} with value {cards[randomIndex].Value}");
        
        // Delete the card at the random index
        gameManagerScript.DeleteCard(randomIndex);
        
        Debug.Log("DeleteRandomCard: Card deleted successfully.");

        if (logo != null)
        {
            StartCoroutine(FadeOutLogo());
        }
    }
    
    private IEnumerator FadeOutLogo()
    {
        if (logo == null) yield break;
        
        float elapsedTime = 0f;
        Color startColor = logo.color;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            
            Color newColor = startColor;
            newColor.a = alpha;
            logo.color = newColor;
            
            yield return null;
        }
        
        // Ensure final transparency is 0
        Color finalColor = logo.color;
        finalColor.a = 0f;
        logo.color = finalColor;
    }
}
