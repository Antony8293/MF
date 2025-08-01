using DG.Tweening;
using UnityEngine;

public class FruitBouncer : MonoBehaviour
{
    public PipeSquashEffect PipeSquashEffect;
    private bool isJumping = true;

    void Start()
    {
        StartCoroutine(JumpLoop());
    }

    private System.Collections.IEnumerator JumpLoop()
    {
        while (isJumping)
        {
            yield return JumpSequence();
        }
    }

    private System.Collections.IEnumerator JumpSequence()
    {
        bool finished = false;
        transform.DOJump(transform.position + Vector3.up * 6f, 0.5f, 1, 3f)
            .OnStart(() =>
            {
                transform.DOScale(new Vector3(1.15f, 0.85f, 1f), 0.12f).SetEase(Ease.OutQuad);
            })
            .OnComplete(() =>
            {
                transform.DOScale(new Vector3(0.85f, 1.15f, 1f), 0.12f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBounce);
                        finished = true;
                    });
            });
        // Đợi cho đến khi hiệu ứng nhảy hoàn thành
        while (!finished)
            yield return null;
    }

    // Khi chuyển scene, bạn có thể set isJumping = false để dừng nhảy
}
