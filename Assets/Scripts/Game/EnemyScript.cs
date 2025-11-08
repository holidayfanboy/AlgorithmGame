using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyData enemyData;
    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyData.activateDeath)
        {
            InitialDeath();
        }
    }

    private void InitialDeath()
    {
        Debug.Log("Enemy has died.");
        animator.SetBool("isDead", true);
    }
}
