using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float time = 2f;
    public float waitingTime = 0.5f; // Time to wait before destroying after fade completes
    private Image image;
    private float elapsedTime = 0f;
    private Color startColor;
    private bool fadeComplete = false;
    
    void Start()
    {
        image = GetComponent<Image>();
        
        if (image == null)
        {
            Debug.LogError("FadeOut: No Image component found on this GameObject!");
            enabled = false;
            return;
        }
        
        startColor = image.color;
    }

    void Update()
    {
        if (image == null) return;
        
        elapsedTime += Time.deltaTime;
        
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / time);
        
        Color newColor = startColor;
        newColor.a = alpha;
        image.color = newColor;
        
        if (elapsedTime >= time && !fadeComplete)
        {
            fadeComplete = true;
            DestroyAfterWait();
        }
    }
    
    private void DestroyAfterWait()
    {
        Destroy(gameObject);
    }
}
