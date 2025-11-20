using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarningDouble : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManagerScript;
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
        gameManagerScript.stageClearGold = 20;
    }

    void Update()
    {
        
    }
}
