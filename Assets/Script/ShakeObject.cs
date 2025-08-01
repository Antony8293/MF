using UnityEngine;
using DG.Tweening;

public class ShakeObject : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;
    private float elapsed = 0f;
    private bool isShaking = false;

    private void OnEnable()
    {
        Booster.booster4 += StartShaking;
        // StartShaking();
    }

    private void OnDisable()
    {
        Booster.booster4 -= StartShaking;
    }

    private void StartShaking()
    {
        Invoke("DelayShaking", 0f);
    }

    private void DelayShaking()
    {
        if (!isShaking && target != null)
        {
            originalPos = transform.localPosition;
            originalRot = transform.localRotation;
            elapsed = 0f;
            isShaking = true;
            ShakeAndRotate();
        }
    }

    public RectTransform target;
    public float moveDistance = 0.2f;     // Di chuyển trái/phải
    public float rotateAngle = 10f;       // Góc xoay Z
    public float stepDuration = 0.1f;     // Thời gian mỗi bước

    public void ShakeAndRotate()
    {
        Vector2 originalPos = target.anchoredPosition;
        Vector3 originalRot = target.localEulerAngles;

        Sequence seq = DOTween.Sequence();

        // Bước 1: Rung mạnh đầu tiên (OutBounce để tạo cảm giác va đập)
        seq.Append(target.DOAnchorPosX(originalPos.x + moveDistance, stepDuration))
           .Join(target.DOLocalRotate(new Vector3(0, 0, -rotateAngle), stepDuration))
           .SetEase(Ease.OutBounce);

        // Bước 2: Rung ngược lại mạnh hơn (InOutElastic để tạo độ đàn hồi)
        seq.Append(target.DOAnchorPosX(originalPos.x - moveDistance, stepDuration * 2))
           .Join(target.DOLocalRotate(new Vector3(0, 0, rotateAngle), stepDuration * 2))
           .SetEase(Ease.InOutElastic);
       
        // Bước 3: Rung nhẹ dần (OutBack để tạo hiệu ứng overshoot nhẹ)
        seq.Append(target.DOAnchorPosX(originalPos.x + moveDistance * 0.5f, stepDuration))
           .Join(target.DOLocalRotate(new Vector3(0, 0, -rotateAngle * 0.5f), stepDuration))
           .SetEase(Ease.OutBack);

        // Bước 4: Về vị trí ban đầu mượt mà (InOutQuart để smooth finish)
        seq.Append(target.DOAnchorPosX(originalPos.x, stepDuration))
           .Join(target.DOLocalRotate(originalRot, stepDuration))
           .SetEase(Ease.InOutQuart);

        // Thêm callback khi hoàn thành
        seq.OnComplete(() =>
        {
            isShaking = false;
            target.anchoredPosition = originalPos;
            target.localEulerAngles = originalRot;

            UIManager.instance.UIScaleShakingBoosterEffect(Const.END_EFFECT);
        });
    }
}
