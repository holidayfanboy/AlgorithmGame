using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSwap : MonoBehaviour
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
            Debug.Log("RandomSwap Skill Activated");
            
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
