using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FindMedian : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManagerScript;
    [SerializeField] private Image logo; // Changed to Image component
    [SerializeField] private float fadeOutDuration = 1f; // Duration for fade out effect
    public bool isActive;
    public int sellPrice;
    
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
        
        // Set logo to fully opaque initially
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
            Debug.Log("FindMedian Skill Activated");
            
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
        
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("Card list is empty or null.");
            yield break;
        }

        // Create a sorted copy of the values to find the median value
        List<int> sortedValues = cards.Select(c => c.Value).OrderBy(v => v).ToList();
        
        int medianValue;
        if (sortedValues.Count % 2 == 0)
        {
            // Even number of cards - take the lower middle value
            medianValue = sortedValues[sortedValues.Count / 2 - 1];
        }
        else
        {
            // Odd number of cards - take the exact middle value
            medianValue = sortedValues[sortedValues.Count / 2];
        }
        
        Debug.Log($"FindMedian: Median value is {medianValue}");

        // Find the card with the median value in the original list
        int medianIndex = -1;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].Value == medianValue)
            {
                medianIndex = i;
                break;
            }
        }

        if (medianIndex == -1)
        {
            Debug.LogWarning("FindMedian: Could not find card with median value.");
            yield break;
        }

        // Calculate the middle position
        int middleIndex = cards.Count / 2;

        // If the median card is already in the middle, do nothing
        if (medianIndex == middleIndex)
        {
            Debug.Log($"FindMedian: Median card (value: {medianValue}) is already at middle position {middleIndex}");
            yield break;
        }

        // Swap the median card to the middle position
        Debug.Log($"FindMedian: Swapping card at index {medianIndex} (value: {medianValue}) to middle position {middleIndex}");
        gameManagerScript.SkillSwapCard(medianIndex, middleIndex);
        // Perform the swap - directly yield the coroutine
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);

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
