using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class Card : MonoBehaviour
{
    public int Value { get; private set; }
    public TMP_Text numberText;
    private Button button;
    private GameManager gameManager;

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

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
