using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FirstStageData firstStageData;
    [SerializeField] private GameObject vfxPrefab; // VFX prefab to spawn
    [SerializeField] private Vector3 vfxOffset = Vector3.zero; // Offset from player position
    [SerializeField] private float vfxScale = 2f; // Scale multiplier for VFX
    [SerializeField] private List<AudioClip> attackClips; // Attack sound effects
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    private Vector3 vfxBasePosition = new Vector3(0.23f, -1.51f, 0f);
    private bool moveAfterAttackRunning = false;
    private float movePlayerAmount = 15.147f; 
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("isAttack", false);
        animator.SetBool("isAttack2", false);
        animator.SetBool("isAttack3", false);
    }

    void Update()
    {
        
    }

    public void StartAttack()
    {
        // Randomly choose one of three attack animations
        int randomAttack = Random.Range(0, 3);
        
        // Reset all attack bools first
        animator.SetBool("isAttack", false);
        animator.SetBool("isAttack2", false);
        animator.SetBool("isAttack3", false);
        
        // Set the randomly selected attack
        switch (randomAttack)
        {
            case 0:
                animator.SetBool("isAttack", true);
                Debug.Log("Boss using Attack 1");
                break;
            case 1:
                animator.SetBool("isAttack2", true);
                Debug.Log("Boss using Attack 2");
                break;
            case 2:
                animator.SetBool("isAttack3", true);
                Debug.Log("Boss using Attack 3");
                break;
        }
    }

    public void PlayerDead()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isAttack2", false);
        animator.SetBool("isAttack3", false);
        animator.SetBool("isDraw", false);
        animator.SetBool("isHurt", false);
        animator.SetBool("isDead", true);
    }

    public void StopAttack()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isAttack2", false);
        animator.SetBool("isAttack3", false);
        animator.SetBool("isDraw", true);
        animator.SetBool("isHurt", false);
    }

    public void Idle()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isAttack2", false);
        animator.SetBool("isAttack3", false);
        animator.SetBool("isDraw", false);
        animator.SetBool("isHurt", false);
    }

    public void StartHurt()
    {
        animator.SetBool("isHurt", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isAttack2", false);
        animator.SetBool("isAttack3", false);
        animator.SetBool("isDraw", false);
        SoundData.PlaySoundFXClip(hurtClip, transform.position, 1.0f);
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
    
    // Wait for the attack animation to complete
    public IEnumerator WaitForAttackAnimation()
    {
        if (animator == null) yield break;
        
        // Wait one frame to ensure animation state has updated
        yield return null;
        
        // Wait while any attack animation is playing
        while (animator.GetBool("isAttack") || animator.GetBool("isAttack2") || animator.GetBool("isAttack3"))
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // Check if we're in any attack state and if the animation has finished
            if ((stateInfo.IsName("Attack") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3")) 
                && stateInfo.normalizedTime >= 1.0f)
            {
                yield break;
            }
            
            yield return null;
        }
    }
    
    // Wait for the death animation to complete
    public IEnumerator WaitForDeathAnimation()
    {
        if (animator == null) yield break;
        
        // Wait one frame to ensure animation state has updated
        yield return null;
        
        SoundData.PlaySoundFXClip(deathClip, transform.position, 1.0f);
        // Wait while death animation is playing
        while (animator.GetBool("isDead"))
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // Check if we're in the death state and if the animation has finished
            if (stateInfo.IsName("Dead") && stateInfo.normalizedTime >= 1.0f)
            {
                yield break;
            }
            
            yield return null;
        }
    }
}
