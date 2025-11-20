using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsertionSort : MonoBehaviour
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
            Debug.Log("InsertionSort Skill Activated");
            
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

        // Find the card with the biggest value from index 0-2
        int maxIndex = 0;
        int maxValue = cards[0].Value;
        int endIndex = Mathf.Min(2, cards.Count - 1); // Check indices 0, 1, 2 (if they exist)
        
        for (int i = 1; i <= endIndex; i++)
        {
            if (cards[i].Value > maxValue)
            {
                maxValue = cards[i].Value;
                maxIndex = i;
            }
        }
        
        Debug.Log($"InsertionSort: Selected card at index {maxIndex} with biggest value {maxValue} from indices 0-{endIndex}");
        
        // Change selected card color to yellow
        cards[maxIndex].ChangeColor(Color.yellow);
        
        int selectedValue = maxValue;
        int currentIndex = maxIndex;
        
        // Compare with all cards from selected position to the end
        for (int i = currentIndex + 1; i < cards.Count; i++)
        {
            Debug.Log($"InsertionSort: Comparing selected value {selectedValue} with card at index {i} (value: {cards[i].Value})");
            
            // If the card at position i has a smaller value than our selected card
            if (cards[i].Value < selectedValue)
            {
                Debug.Log($"InsertionSort: Card at index {i} (value: {cards[i].Value}) is smaller than selected value {selectedValue}. Swapping...");
                
                gameManagerScript.SkillSwapCard(currentIndex, i);
                yield return new WaitUntil(() => !gameManagerScript.isSwapping);
                
                // Update the current position of our selected card
                currentIndex = i;
            }
        }
        
        // Change selected card color back to red when done
        cards[currentIndex].ChangeColor(Color.red);
        
        Debug.Log($"InsertionSort: Completed. Selected card ended at index {currentIndex}");

        // Start fading out the logo
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
