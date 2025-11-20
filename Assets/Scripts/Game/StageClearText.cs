using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageClearText : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private float fadeInDuration = 1f; // Duration of fade-in effect
    
    void Start()
    {
        // Get TMP_Text component if not assigned
        if (textMesh == null)
        {
            textMesh = GetComponent<TMP_Text>();
        }
        
        if (textMesh != null)
        {
            // Start with transparent text
            Color color = textMesh.color;
            color.a = 0f;
            textMesh.color = color;
            
            // Start fade-in effect
            StartCoroutine(FadeInText());
        }
    }
    
    private IEnumerator FadeInText()
    {
        if (textMesh == null) yield break;
        
        float elapsedTime = 0f;
        Color startColor = textMesh.color;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            
            Color newColor = startColor;
            newColor.a = alpha;
            textMesh.color = newColor;
            
            yield return null;
        }
        
        // Ensure final alpha is 1 (fully opaque)
        Color finalColor = textMesh.color;
        finalColor.a = 1f;
        textMesh.color = finalColor;
    }
}
