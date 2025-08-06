using UnityEngine;
using DG.Tweening;

public class ShakeObject : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        // Cache animator component
        animator = GetComponent<Animator>();
    }

    public void OnAnimationEnd()
    {
        animator.enabled = false; // Disable animator after one animation cycle
        UIManager.instance?.UIScaleShakingBoosterEffect(Const.END_EFFECT);
    }
}
