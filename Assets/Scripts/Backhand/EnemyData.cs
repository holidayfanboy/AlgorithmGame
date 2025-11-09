using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public bool activateDeath;


    void OnEnable()
    {
        activateDeath = false;
    }

    void Update()
    {

    }


    public void ActivateDeath()
    {
        activateDeath = true;

    }

    public void DeactivateDeath()
    {
        activateDeath = false;
    }
}
