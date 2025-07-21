using UnityEngine;
using DG.Tweening;

public class PipeSquashEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Cấu hình squash")]
    [Tooltip("Thời gian hiệu ứng squash")]
    public float duration = 0.05f;

    [Tooltip("Tỉ lệ squash theo chiều ngang")]
    public float squashX = 1.05f;
    public float squashY = 0.95f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerPipeSquash()
    {
        var pipeScale = gameObject.transform.localScale;

        float dx = squashX;
        float dy = squashY;

        Sequence sequence = DOTween.Sequence().SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        Vector3 squashScale = new Vector3(
            pipeScale.x * dx,
            pipeScale.y * dy,
            pipeScale.z
        );

        Vector3 stretchScale = new Vector3(
            pipeScale.x * dy,
            pipeScale.y * dx,
            pipeScale.z
        );

        sequence.Append(transform.DOScale(squashScale, duration).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(stretchScale, duration).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(pipeScale, duration).SetEase(Ease.OutQuad));
    }
    public void TriggerDraggingSquash()
    {
        var pipeScale = gameObject.transform.localScale;

        float dx = squashX;
        float dy = squashY;

        Sequence sequence = DOTween.Sequence();

        Vector3 squashScale = new Vector3(
            pipeScale.x * dx,
            pipeScale.y * dy,
            pipeScale.z
        );

        Vector3 stretchScale = new Vector3(
            pipeScale.x * dy,
            pipeScale.y * dx,
            pipeScale.z
        );

        sequence.Append(transform.DOScale(stretchScale, duration).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(squashScale, duration).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(pipeScale, duration).SetEase(Ease.OutQuad));
    }
}
