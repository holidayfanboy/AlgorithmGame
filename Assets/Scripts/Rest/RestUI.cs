using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class RestUI : MonoBehaviour
{
    public TMP_Text roundText;
    [SerializeField] FirstStageData firstStageData;
    public int maxStage;
    public int currentStage;
    public Slider slider;
    [SerializeField] private float sliderSpeed = 1f;     
    private float targetSliderValue;
    
    void Start()
    {
        currentStage = firstStageData.currentStage;
        maxStage = firstStageData.GiveMaxStageSize();
        if (firstStageData == null)
        {
            Debug.LogWarning("RestUI: FirstStageData reference not set. Attempting to find in scene.");
            firstStageData = FindObjectOfType<FirstStageData>();
        }
        
        targetSliderValue = (float)currentStage / maxStage;
        slider.value = targetSliderValue; 
        
        firstStageData.IncreaseStage();
        currentStage = firstStageData.currentStage; // Update to new stage
        roundText.text = "1-" + currentStage.ToString();
        
        targetSliderValue = (float)currentStage / maxStage;
    }

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetSliderValue, Time.deltaTime * sliderSpeed);
    }
}
