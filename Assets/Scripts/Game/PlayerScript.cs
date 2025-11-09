using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FirstStageData firstStageData;
    private bool moveAfterAttackRunning = false;
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("isAttack", false);
    }

    void Update()
    {
        
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

    public void PlayerDead()
    {
        animator.SetBool("isDead", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isDraw", false);
        animator.SetBool("isHurt", false);
    }

    public void StopAttack()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isDraw", true);
        animator.SetBool("isHurt", false);
    }

    public void Idle()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isDraw", false);
        animator.SetBool("isHurt", false);
    }

    public void StartHurt()
    {
        animator.SetBool("isHurt", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isDraw", false);
    }
    private IEnumerator MoveAfterAttack()
    {
        moveAfterAttackRunning = true;
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(transform.position.x + 15.147f, transform.position.y, transform.position.z);
        CheckWinCondition();
        moveAfterAttackRunning = false;
    }

    private void CheckWinCondition()
    {
        
        if (firstStageData.stageSuccess == true)
        {
            Debug.Log("Won");
            StopAttack();
        }
        else
        {
            Debug.Log("Lost");
            StartHurt();
        }
    }
}
