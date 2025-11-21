using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class Card : MonoBehaviour
{
    public int Value { get; private set; }

    public enum ColorType
    {
        Red,
        Blue,
        Green
    }

    public ColorType colorType;
    public TMP_Text numberText;
    private Button button;
    private GameManager gameManager;
    [SerializeField] private GameObject outLine;
    [SerializeField] private Image cardImage; // Main card background image
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    public void SetValue(int v)
    {
        Value = v;
        if (numberText != null)
            numberText.text = v.ToString();
    }

    public void UpdateCardColor()
    {
        if (cardImage == null) return;
        
        switch (colorType)
        {
            case ColorType.Red:
                cardImage.color = new Color(1f, 0.3f, 0.3f); // Light red
                break;
            case ColorType.Blue:
                cardImage.color = new Color(0.3f, 0.5f, 1f); // Light blue
                break;
            case ColorType.Green:
                cardImage.color = new Color(0.3f, 1f, 0.3f); // Light green
                break;
        }
    }

    private void OnButtonClick()
    {
        if (gameManager != null)
        {
            gameManager.OnItemClicked(gameObject);
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }

    public void ChangeColor(Color newColor)
    {
        outLine.GetComponent<Image>().color = newColor;
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
