using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class SquashStretch : MonoBehaviour
{
    [Header("Cấu hình squash")]
    public float squashX = 1.05f;
    public float squashY = 0.95f;
    public float duration = 0.05f;
    public float minVelocity = 1f;

    [Header("Cooldown")]
    public float cooldown = 0.5f;

    [Header("Tham chiếu")]
    public Transform visual; // GameObject con chứa SpriteRenderer

    private bool isCoolingDown = false;
    private Vector3 originalScale;
    private CircleComponent circleComp;

    [Header("Giới hạn")]
    public float maxSquashStretchRatio = 1.05f;

    void Start()
    {

        // Nếu chưa gán visual thủ công, tự tìm con tên "CircleSprite"
        if (visual == null)
        {
            Transform found = transform.Find("CircleSprite");
            if (found != null)
            {
                visual = found;
            }
            else
            {
                Debug.LogWarning($"[{name}] Không tìm thấy object con tên 'CircleSprite'. Hãy gán thủ công nếu cần.");
            }
        }

        // Lấy CircleComponent
        circleComp = GetComponent<CircleComponent>();
        if (circleComp == null)
        {
            Debug.LogError($"[{name}] Không tìm thấy CircleComponent để lấy targetScale.");
        }

        // Tìm và gán vật liệu vật lý tên "BouncyMat"
        if (circleComp != null && circleComp.transform.childCount > 0)
        {
            // Tìm CircleCollider2D ở GameObject con (visual)
            CircleCollider2D childCollider = circleComp.GetComponentInChildren<CircleCollider2D>();

            if (childCollider != null)
            {
                PhysicsMaterial2D bouncyMat = Resources.Load<PhysicsMaterial2D>("Materials/Physics/BouncyMat");
                if (bouncyMat != null)
                {
                    childCollider.sharedMaterial = bouncyMat;
                }
                else
                {
                    Debug.LogWarning($"[{name}] Không tìm thấy PhysicsMaterial2D tên 'BouncyMat' trong Resources.");
                }
            }
            else
            {
                Debug.LogWarning($"[{name}] Không tìm thấy CircleCollider2D trong con của circleComp.");
            }
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (isCoolingDown || visual == null) return;
    //     float velocity = collision.relativeVelocity.magnitude;
    //     if (velocity < minVelocity) return;

    //     Vector2 normal = collision.GetContact(0).normal;
    //     StartCoroutine(RotateAndSquash(normal, velocity, collision.GetContact(0).point));
    // }

    private void CalculateSquashFactors(float velocity, out float squashXOut, out float squashYOut)
    {
        float velocityFactor = Mathf.Clamp(velocity / 6f, 0.95f, 1.2f);
        float dx = Mathf.Clamp(squashX * velocityFactor, 0.9f, 1.3f);
        float dy = Mathf.Clamp(squashY * (2f - velocityFactor), 0.7f, 0.95f);

        float ratio = dx / dy;
        if (ratio > maxSquashStretchRatio)
        {
            float sqrtMax = Mathf.Sqrt(maxSquashStretchRatio);
            float balanced = Mathf.Sqrt(dx * dy);
            dx = balanced * sqrtMax;
            dy = balanced / sqrtMax;
        }

        squashXOut = dx;
        squashYOut = dy;
    }

    public void TriggerSquash(Vector2 normal, float velocity, Vector2 contactPoint, bool isFirstCollision = false)
    {
        if (isCoolingDown || visual == null) return;
        StartCoroutine(RotateAndSquash(normal, velocity, contactPoint, isFirstCollision));
    }
    
    private System.Collections.IEnumerator RotateAndSquash(Vector2 normal, float velocity, Vector2 contactPoint, bool isFirstCollision = false)
    {
        if (transform.localScale.x < 0.01f || transform.localScale.y < 0.01f)
        {
            Debug.LogWarning($"{name} scale quá nhỏ, huỷ squash");
            yield break;
        }

        isCoolingDown = true;

        // Tách visual ra để tránh bị xoay
        visual.SetParent(null);

        Quaternion originalRotation = transform.rotation;
        // Xoay object cha theo hướng normal
        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);


        // Dịch transform về phía sau theo normal, tính từ vị trí pivot visual
        if (!isFirstCollision)
        {
            // Lưu lại vị trí pivot gốc của visual
            Vector3 pivotWorldPos = visual.position;

            // Tính offset (theo collider hoặc sprite)
            float offset = Vector3.Distance(visual.position, contactPoint);
            transform.position = pivotWorldPos + (Vector3)(normal.normalized * offset);
            Debug.DrawLine(pivotWorldPos, transform.position, Color.red, 1f);
        }


        // Gắn lại visual về cha (vị trí không đổi)
        visual.SetParent(transform, worldPositionStays: true);

        Vector3 baseScale = circleComp != null ? circleComp.targetScale : transform.localScale;
        if (baseScale == Vector3.zero)
        {
            Debug.LogWarning($"{name} targetScale là 0, không thể squash/stretch");
            yield break;
        }

        CalculateSquashFactors(velocity, out float dx, out float dy);

        Sequence sequence = DOTween.Sequence().SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        Vector3 squashScale = new Vector3(
            baseScale.x * dx,
            baseScale.y * dy,
            baseScale.z
        );

        Vector3 stretchScale = new Vector3(
            baseScale.x * dy,
            baseScale.y * dx,
            baseScale.z
        );

        sequence.Append(transform.DOScale(squashScale, duration).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(stretchScale, duration).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(baseScale, duration).SetEase(Ease.OutQuad));
        sequence.OnComplete(() =>
        {
            /** Set pivot về vị trí gốc của visual */
                // Tách visual ra để tránh bị xoay/di chuyển
                // visual.SetParent(null);

                // transform.position = visual.position; // Trả về vị trí gốc của visual => merge đúng vị trí // Uncomment bị di chuyển không hợp lý

                // sau khi squash/stretch:
                // transform.rotation = originalRotation;

                // visual.SetParent(transform, worldPositionStays: true);
            Invoke(nameof(ResetCooldown), cooldown);
        });

        yield return null;
    }

    void ResetCooldown()
    {
        isCoolingDown = false;
    }
}
