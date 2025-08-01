using UnityEngine;
using DG.Tweening;

public class ShakeObject : MonoBehaviour
{
    private bool isShaking = false;
    private Animator animator;

    private void OnEnable()
    {
        Booster.booster4 += StartShaking;
        // StartShaking();
    }

    private void Start()
    {
        // Cache animator component
        animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        Booster.booster4 -= StartShaking;
    }

    private void StartShaking()
    {
        if (animator != null)
        {
            isShaking = true;
            animator.enabled = true; // Enable the animator to start shaking
            // StartCoroutine(WaitForAnimationEnd());
        }
    }

    // // Simple coroutine to wait for animation end
    // private System.Collections.IEnumerator WaitForAnimationEnd()
    // {
    //     if (animator == null) yield break;

    //     // Wait for at least one frame to ensure animation starts
    //     yield return new WaitForEndOfFrame();

    //     // Wait until one complete animation cycle finishes
    //     while (animator.enabled)
    //     {
    //         AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    //         // Check if animation finished one complete cycle (normalizedTime >= 1.0 and not transitioning)
    //         if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
    //         {
    //             break; // One animation cycle completed
    //         }

    //         yield return null; // Wait for next frame
    //     }

    //     // Animation completed - run callback
    //     OnAnimationEnd();
    // }

    // private void OnAnimationEnd()
    // {
    //     // Tắt animator sau khi chạy xong 1 lần animation clip
    //     if (animator != null)
    //     {
    //         animator.enabled = false; // Disable animator after one animation cycle
    //     }

    //     isShaking = false;

    //     // Chạy callback sau khi animation hoàn thành
    //     UIManager.instance?.UIScaleShakingBoosterEffect(Const.END_EFFECT);
    // }

    public void OnAnimationEnd()
    {
        if (isShaking)
        {
            isShaking = false;
            UIManager.instance?.UIScaleShakingBoosterEffect(Const.END_EFFECT);
            animator.enabled = false; // Disable animator after one animation cycle
        }
    }
}
