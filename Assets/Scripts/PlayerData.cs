using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int PlayerHandRange;
    public int PlayerHealth;

    void Awake()
    {
        PlayerHandRange = 1;
        PlayerHealth = 3;
    }

    void Update()
    {

    }
    
    public int getPlayerHand()
    {
        return PlayerHandRange;
    }
}
