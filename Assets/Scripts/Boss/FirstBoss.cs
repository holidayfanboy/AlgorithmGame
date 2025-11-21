using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // Initialize animation parameters to false
        animator.SetBool("isAttack", false);
        animator.SetBool("isHurt", false);
    }

    void Update()
    {
        
    }
    
    public void StartAttack()
    {
        StartCoroutine(AttackCoroutine());
    }
    
    public void StartHurt()
    {
        StartCoroutine(HurtCoroutine());
    }
    
    private IEnumerator AttackCoroutine()
    {
        // Set attack to true
        animator.SetBool("isAttack", true);
        Debug.Log("Boss Attack Started");
        
        // Wait 1 second
        yield return new WaitForSeconds(1f);
        
        // Set attack back to false
        animator.SetBool("isAttack", false);
        Debug.Log("Boss Attack Ended");
    }
    
    private IEnumerator HurtCoroutine()
    {
        // Set hurt to true
        animator.SetBool("isHurt", true);
        Debug.Log("Boss Hurt Started");
        
        // Wait 1 second
        yield return new WaitForSeconds(1f);
        
        // Set hurt back to false
        animator.SetBool("isHurt", false);
        Debug.Log("Boss Hurt Ended");
    }
}
