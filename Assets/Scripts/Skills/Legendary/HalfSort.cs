using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HalfSort : MonoBehaviour
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
            Debug.Log("SortLeftHalf Skill Activated");
            
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
        if (logo != null)
        {
            StartCoroutine(FadeOutLogo());
        }
        
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

        // Find the middle index
        int middleIndex = cards.Count / 2;
        Debug.Log($"SortLeftHalf: Middle index is {middleIndex}. Sorting left side (indices 0 to {middleIndex - 1})");
        Color darkGreen = new Color(0f, 0.5f, 0f); 
        // If there's nothing on the left side, exit
        if (middleIndex == 0)
        {
            Debug.Log("SortLeftHalf: No cards on the left side to sort.");
            yield break;
        }

        // Change middle card to yellow
        cards[middleIndex].ChangeColor(Color.yellow);
        
        // Change all left side cards to green
        for (int i = 0; i < middleIndex; i++)
        {
            cards[i].ChangeColor(darkGreen);
        }

        // Perform insertion sort on the left side (indices 0 to middleIndex - 1)
        for (int i = 1; i < middleIndex; i++)
        {
            int currentValue = cards[i].Value;
            int currentIndex = i;
            
            Debug.Log($"SortLeftHalf: Processing card at index {i} with value {currentValue}");
            
            // Compare with cards to the left and swap if needed
            for (int j = i - 1; j >= 0; j--)
            {
                if (cards[currentIndex].Value < cards[j].Value)
                {
                    Debug.Log($"SortLeftHalf: Swapping index {currentIndex} (value: {cards[currentIndex].Value}) with index {j} (value: {cards[j].Value})");
                    
                    gameManagerScript.SkillSwapCard(currentIndex, j);
                yield return new WaitUntil(() => !gameManagerScript.isSwapping);
                    
                    currentIndex = j;
                }
                else
                {
                    break; // Found correct position
                }
            }
        }
        
        // Change all cards back to red after sorting is complete
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].ChangeColor(Color.red);
        }
        
        Debug.Log("SortLeftHalf: Completed sorting left side.");

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
