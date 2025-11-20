using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinarySort : MonoBehaviour
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
            Debug.Log("BinarySort Skill Activated");
            
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

        // Start fading out the logo
        if (logo != null)
        {
            StartCoroutine(FadeOutLogo());
        }
        
        Debug.Log("BinarySort: Starting binary heap sort");
        
        // Step 1: Sort first 3 cards (indices 0,1,2)
        yield return StartCoroutine(SortFirstThree(cards));
        
        // Step 2: Process remaining cards
        // We treat index as starting from 1 for binary heap logic
        // But actual array is 0-indexed
        for (int i = 1; i < cards.Count; i++) // i represents heap index (1-based logic)
        {
            int leftChildIndex = (i * 2); // Left child in 1-based logic
            int rightChildIndex = (i * 2) + 1; // Right child in 1-based logic
            
            // Convert back to 0-based for array access
            int currentIndex = i;
            int leftIndex = leftChildIndex;
            int rightIndex = rightChildIndex;
            
            // Check if children exist in the list
            if (leftIndex >= cards.Count)
            {
                Debug.Log($"BinarySort: No more children for index {i}");
                break;
            }
            
            Debug.Log($"BinarySort: Processing index {i} (actual: {currentIndex}), checking children at {leftIndex}" + 
                     (rightIndex < cards.Count ? $" and {rightIndex}" : ""));
            
            // Highlight current and children in yellow
            cards[currentIndex].ChangeColor(Color.yellow);
            cards[leftIndex].ChangeColor(Color.yellow);
            if (rightIndex < cards.Count)
            {
                cards[rightIndex].ChangeColor(Color.yellow);
            }
            yield return new WaitForSeconds(0.5f);
            
            // Find the smallest among current, left, and right (if exists)
            int smallestIndex = currentIndex;
            int smallestValue = cards[currentIndex].Value;
            
            if (cards[leftIndex].Value < smallestValue)
            {
                smallestIndex = leftIndex;
                smallestValue = cards[leftIndex].Value;
            }
            
            if (rightIndex < cards.Count && cards[rightIndex].Value < smallestValue)
            {
                smallestIndex = rightIndex;
                smallestValue = cards[rightIndex].Value;
            }
            
            // Mark the smallest card in magenta
            cards[smallestIndex].ChangeColor(Color.magenta);
            yield return new WaitForSeconds(0.5f);
            
            // Swap if needed
            if (smallestIndex != currentIndex)
            {
                Debug.Log($"BinarySort: Swapping index {currentIndex} (value: {cards[currentIndex].Value}) " +
                         $"with index {smallestIndex} (value: {cards[smallestIndex].Value})");
                
                gameManagerScript.SkillSwapCard(currentIndex, smallestIndex);
                
                yield return new WaitUntil(() => !gameManagerScript.isSwapping);
                yield return new WaitForSeconds(0.5f);
            }
            
            // Reset colors to red
            cards[currentIndex].ChangeColor(Color.red);
            cards[leftIndex].ChangeColor(Color.red);
            if (rightIndex < cards.Count)
            {
                cards[rightIndex].ChangeColor(Color.red);
            }
        }
        
        // Reset all card colors
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].ChangeColor(Color.red);
        }
        
        Debug.Log("BinarySort: Completed");
    }
    
    private IEnumerator SortFirstThree(List<Card> cards)
    {
        if (cards.Count < 3)
        {
            Debug.LogWarning("BinarySort: Less than 3 cards, skipping first three sort");
            yield break;
        }
        
        Debug.Log("BinarySort: Sorting first three cards (indices 0, 1, 2)");
        
        // Highlight first 3 cards in yellow
        for (int i = 0; i < 3; i++)
        {
            cards[i].ChangeColor(Color.yellow);
        }
        yield return new WaitForSeconds(0.5f);
        
        // Find the smallest among first 3 cards
        int smallestIndex = 0;
        int smallestValue = cards[0].Value;
        
        for (int i = 1; i < 3; i++)
        {
            if (cards[i].Value < smallestValue)
            {
                smallestIndex = i;
                smallestValue = cards[i].Value;
            }
        }
        
        // Mark the smallest card in magenta
        cards[smallestIndex].ChangeColor(Color.magenta);
        yield return new WaitForSeconds(0.5f);
        
        // Swap smallest to index 0 if needed
        if (smallestIndex != 0)
        {
            Debug.Log($"BinarySort: Moving smallest card from index {smallestIndex} to index 0");
            
            gameManagerScript.SkillSwapCard(0, smallestIndex);
            
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);
            yield return new WaitForSeconds(0.5f);
        }
        
        // Reset colors to red
        for (int i = 0; i < 3; i++)
        {
            cards[i].ChangeColor(Color.red);
        }
        
        Debug.Log("BinarySort: First three sorted");
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
