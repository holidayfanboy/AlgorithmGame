using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeFreeze : MonoBehaviour
{
    [SerializeField] private TimerScript timerScript;
    [SerializeField] private Image logo; // UI Image for logo
    [SerializeField] private float fadeOutDuration = 1f; // Duration for fade out effect
    [SerializeField] private float freezeDuration = 3f; // Duration to freeze timer
    public bool isActive;
    public int sellPrice;
    
    private RectTransform rectTransform;
    
    void Awake()
    {
        timerScript = FindObjectOfType<TimerScript>();
        rectTransform = GetComponent<RectTransform>();
        
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
            Debug.Log("TimeFreeze Skill Activated");
            
            // Set logo to fully opaque when activated
            if (logo != null)
            {
                Color color = logo.color;
                color.a = 1f; // Set to 255/255 = 1.0
                logo.color = color;
            }
            
            
            StartCoroutine(ActivateSkillCoroutine());
            isActive = false; 
        }
    }
    
    private IEnumerator ActivateSkillCoroutine()
    {
        // Start fading out the logo
        if (logo != null)
        {
            StartCoroutine(FadeOutLogo());
        }
        
        Debug.Log($"TimeFreeze: Freezing timer for {freezeDuration} seconds");
        
        // Freeze the timer
        timerScript.isTimerFrozen = true;
        
        // Wait for freeze duration
        yield return new WaitForSeconds(freezeDuration);
        
        // Unfreeze the timer
        timerScript.isTimerFrozen = false;
        
        Debug.Log("TimeFreeze: Timer unfrozen");
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
