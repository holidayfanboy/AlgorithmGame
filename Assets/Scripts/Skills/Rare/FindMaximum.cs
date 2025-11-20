using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindMaximum : MonoBehaviour
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
            Debug.Log("FindMaximum Skill Activated");
            
            // Set logo to fully opaque when activated
            if (logo != null)
            {
                Color color = logo.color;
                color.a = 1f; // Set to 255/255 = 1.0
                logo.color = color;
            }
            
            StartCoroutine(ActivateSkillCoroutine(gameManagerScript.spawnedCards));
            isActive = false; // prevent multiple activations
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
        
        int maxIndex = 0;
        int maxValue = cards[0].Value;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;
            if (cards[i].Value > maxValue)
            {
                maxValue = cards[i].Value;
                maxIndex = i;
            }
        }

        int lastIndex = cards.Count - 1;

        // If the maximum card is not already at the last position, swap it
        if (maxIndex != lastIndex)
        {
            gameManagerScript.SkillSwapCard(lastIndex, maxIndex); // Changed from SwapCard
            Debug.Log($"Maximum value card ({maxValue}) moved to position {lastIndex}");
            
            // Wait for the swap animation to complete
            yield return new WaitUntil(() => !gameManagerScript.isSwapping);
        }
        else
        {
            Debug.Log($"Maximum value card ({maxValue}) is already at position {lastIndex}");
        }

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
