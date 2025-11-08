using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool moveAfterAttackRunning = false;
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        StartAttack();
    }

    public void StartAttack()
    {
        // Only trigger when entering attack state to avoid scheduling multiple moves per frame
        if (!animator.GetBool("isAttack"))
        {
            animator.SetBool("isAttack", true);

            if (!moveAfterAttackRunning)
            {
                StartCoroutine(MoveAfterAttack());
            }
        }
    }

    public void StopAttack()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isDraw", true);
    }

    public void StartHurt()
    {
        animator.SetBool("isHurt", true);
        animator.SetBool("isAttack", false);
    }
    
    public void BacktoIdle()
    {
        animator.SetBool("isHurt", false);
        animator.SetBool("isDraw", false);
    }

    private IEnumerator MoveAfterAttack()
    {
        moveAfterAttackRunning = true;
        yield return new WaitForSeconds(2.5f);
        transform.position = new Vector3(7.95f, -1.16f, 0f);
        moveAfterAttackRunning = false;
    }
}
