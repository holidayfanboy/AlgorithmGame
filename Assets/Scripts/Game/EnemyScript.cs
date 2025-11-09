using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyData enemyData;
    private bool hasTriggeredDeath = false;

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
        if (enemyData.activateDeath && !hasTriggeredDeath)
        {
            InitialDeath();
        }
    }

    private void InitialDeath()
    {
        hasTriggeredDeath = true;
        animator.SetBool("isDead", true);
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }
}
