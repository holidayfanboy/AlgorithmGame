using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
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
        animator.SetBool("isAttack", false);
        animator.SetBool("isDraw", false);
        animator.SetBool("isHurt", false);
        animator.SetBool("isDead", true);
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
        SoundData.PlaySoundFXClip(hurtClip, transform.position, 1.0f);
    }
    private IEnumerator MoveAfterAttack()
    {
        moveAfterAttackRunning = true;
        yield return new WaitForSeconds(2f);
        
        // Spawn VFX at player position
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, vfxBasePosition, Quaternion.identity);
            vfx.transform.localScale *= vfxScale;
        }

        if (attackClips != null && attackClips.Count > 0)
        {
            SoundData.PlaySoundFXClip(attackClips.ToArray(), transform.position, 1.0f);
        }
        
        transform.position = new Vector3(transform.position.x + movePlayerAmount, transform.position.y, transform.position.z);
        vfxBasePosition.x += movePlayerAmount;
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
    
    // Wait for the attack animation to complete
    public IEnumerator WaitForAttackAnimation()
    {
        if (animator == null) yield break;
        
        // Wait one frame to ensure animation state has updated
        yield return null;
        
        // Wait while attack animation is playing
        while (animator.GetBool("isAttack"))
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // Check if we're in the attack state and if the animation has finished
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
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
