using DG.Tweening;
using UnityEngine;

public class FruitBouncer : MonoBehaviour
{
    public PipeSquashEffect PipeSquashEffect;
    public static bool isJumping = true;
    Vector3 originalScale;
    void Start()
    {
        originalScale = gameObject.transform.localScale;
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
        Vector3 startPos = transform.position;
        Vector3 groundPos = startPos + Vector3.down * 3f; // Rơi xuống đất
        Vector3 jumpPos = startPos; // Vị trí nhảy lên lại

        // 1. Rơi xuống
        transform.DOMove(groundPos, 0.5f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                // 2. Squash khi chạm đất
                transform.DOScale(new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z), 0.12f).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        // 3. Nhảy lên lại
                        transform.DOScale(originalScale, 0.18f).SetEase(Ease.OutBounce);
                        transform.DOMove(jumpPos, 0.5f).SetEase(Ease.OutQuad)
                            .OnComplete(() => { finished = true; });
                    });
            });
        // Đợi cho đến khi hiệu ứng hoàn thành
        while (!finished)
            yield return null;
    }

    // Khi chuyển scene, bạn có thể set isJumping = false để dừng nhảy
}
