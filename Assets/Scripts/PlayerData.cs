using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int PlayerHandRange;
    public int SkillHand;
    

    void Awake()
    {
        PlayerHandRange = 1;
    }

    void Update()
    {

    }
    
    public int getPlayerHand()
    {
        return PlayerHandRange;
    }
}
