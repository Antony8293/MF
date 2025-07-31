using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CircleComponent : MonoBehaviour
{
    // public static event Action<CircleComponent> AddCircleQueueToDestroy;

    public static event Action<String, Vector3, Color, int> PracticeEffect;

    public static event Action<CircleComponent, CircleComponent, UnityEngine.Vector3> OnCircleMerged;

    public Action OnUpgrade;

    public static event Action<UnityEngine.Object> AfterUpgrade;

    public Vector3 targetScale;
    public bool isAutoScale = true; // Flag để kiểm soát auto scale = quả sinh ra bởi merge
    Vector3 currentScale = Vector3.zero;

    private Vector2 contactPoint;
    bool isMerging = false;
    [SerializeField] private int level;
    readonly float maxMass = 1f; // Giới hạn khối lượng tối đa của CircleComponent

    public void SetTargetScale(Vector3 scale)
    {
        targetScale = scale;
    }
    public int Level => level;
    public void SetLevel(int l) => level = l;
    [SerializeField] public AnimalEvolutionTree evolutionTree;
    public void SetEvolutionTree(AnimalEvolutionTree tree) => evolutionTree = tree;

    private Material outlineMaterial;

    private AnimalData data;

    private Material stretchMaterial;
    private Rigidbody2D _rigidbody;
    private MoveCircle _moveCircle;
    private Animator _animator;
    private bool canStretch = false;

    public bool isFirstCollision = true;
    public bool isOverLineTriggered = false;

    // Reference để lưu trigger collider được tạo ra
    private CircleCollider2D triggerMergeCollider;

    public AnimationClip deadClip;
    public AnimationClip idleClip;
    public AnimationClip mergeClip;
    [SerializeField]
    public bool isAnimated = true;
    public bool hasTriggeredDead = false;
    public float timeTriggerMerge = 0f;
    public bool isMergeAnimationPlaying = false;
    public GameObject mergingNeighbor;
    public EyesControl _eyesControl;

    private GameObject aimingGO;

    private void OnEnable()
    {
        OnUpgrade += ChangeToUpgrade;
        //
        _rigidbody = GetComponent<Rigidbody2D>();
        _moveCircle = GetComponent<MoveCircle>();

        // Gán Anim
        _animator = GetComponentInChildren<Animator>();
        if (isAnimated)
        {
            // Bật
            var overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

            overrideController["eyefollow_idle_f1"] = idleClip;
            overrideController["eyefollow_merge_f1"] = mergeClip;
            overrideController["eyefollow_dead_f1"] = deadClip;

            _animator.runtimeAnimatorController = overrideController;

        }
        else
        {
            // Tắt anim
            _animator.enabled = false;
        }

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col != null)
        {
            PhysicsMaterial2D bounceMaterial = Resources.Load<PhysicsMaterial2D>("Materials/Physics/BouncyMat");


            if (bounceMaterial != null)
            {
                col.sharedMaterial = bounceMaterial;
            }
            else
            {
                Debug.LogWarning("BouncyMaterial not found in Resources folder!");
            }
        }

        _eyesControl = GetComponentInChildren<EyesControl>();

        // Cache the reference to the AimingComponent GameObject
        var aimingComponent = GetComponentInChildren<AimingComponent>();
        if (aimingComponent != null)
        {
            aimingGO = aimingComponent.gameObject;
            aimingGO.SetActive(false); // Ẩn AimingComponent khi không cần thiết
        }
        else
        {
            Debug.LogWarning("AimingComponent not found in CircleComponent!");
        }
        // aimingGO = aimingTransform != null ? aimingTransform.gameObject : null;
    }

    private void Start()
    {

        // Chỉ reset scale về 0 nếu được cho phép auto scale
        if (isAutoScale)
        {
            transform.localScale = Vector3.zero; // Khi thả auto scale về 0 - bug
        }

        // 1. Kiểm tra nếu evolutionTree đã được gán
        if (evolutionTree == null)
        {
            evolutionTree = GameManager.instance.evolutionTree;
            data = evolutionTree.GetLevelData(Level - 1);

            // Chỉ auto scale nếu được cho phép
            if (isAutoScale)
            {
                transform.DOScale(targetScale, 0.25f);
            }
            else
            {
                // Nếu không auto scale thì tạo collider để trigger merge
                // Debug.Log($"[{name}] Không auto scale, tạo collider để trigger merge.");
                CreateTriggerMergeCollider();

                // Hiệu ứng merge default khi sinh
                timeTriggerMerge = 2f; // Thời gian trigger merge
                mergingNeighbor = gameObject; // Đặt mergingNeighbor là chính nó
            }

            transform.GetComponent<Rigidbody2D>().mass = maxMass / evolutionTree.GetMaxLevel() * Level;
            // ApplyFixedOutlineWidth();
        }

        if (_eyesControl != null)
        {
            _eyesControl.SetTargetEyes(GameManager.instance.draggingCircleGO.transform);
        }
        else
        {
            Debug.LogWarning("EyesControl not found in CircleComponent!");
        }

    }

    private void CreateTriggerMergeCollider()
    {
        // Tạo collider để trigger merge
        triggerMergeCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerMergeCollider.isTrigger = true;

        // Tính radius dựa trên targetScale thay vì scale hiện tại (vì lúc này scale = 0)
        var childCollider = gameObject.GetComponentInChildren<CircleCollider2D>();
        if (childCollider != null)
        {
            float baseRadius = childCollider.radius;
            float scaleMultiplier = Mathf.Max(targetScale.x, targetScale.y);
            triggerMergeCollider.radius = baseRadius * scaleMultiplier * 10f;
        }
        else
        {
            triggerMergeCollider.radius = 2f; // Fallback value
        }

        triggerMergeCollider.offset = Vector2.zero; // Vị trí tùy chỉnh nếu cần
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log($"[{name}] OnTriggerEnter2D for [{collision.gameObject.name}]");
        if (triggerMergeCollider != null)
        {
            var circleComponent = collision.gameObject.GetComponentInParent<CircleComponent>();
            if (circleComponent != null)
            {
                // Cho phép reset timer ngay cả khi đang chạy animation merge
                circleComponent.timeTriggerMerge = 2f;
                circleComponent.mergingNeighbor = gameObject;
                // Debug.Log($"[{name}] Set timeTriggerMerge = 2f for [{collision.gameObject.name}]");
            }
            // Delay 0.5s trước khi xóa trigger collider
            StartCoroutine(DelayDestroyTriggerCollider(2f));
        }
    }

    private IEnumerator DelayDestroyTriggerCollider(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);

        // Xóa trigger collider sau delay
        if (triggerMergeCollider != null)
        {
            Destroy(triggerMergeCollider);
            triggerMergeCollider = null;
        }
    }

    private void Update()
    {
        if (!_moveCircle.enabled && !isFirstCollision && !isOverLineTriggered && !hasTriggeredDead)
        {
            hasTriggeredDead = true;
            _animator.SetTrigger("TriggerDead");
            PlayDeadEffect();
        }

        if (!_moveCircle.enabled && !isFirstCollision && isOverLineTriggered && hasTriggeredDead)
        {
            hasTriggeredDead = false;
            _animator.SetTrigger("TriggerIdle");
            StopDeadEffect();
        }

        if (timeTriggerMerge > 0f)
        {
            // Trigger merge animation chỉ một lần khi bắt đầu
            if (!isMergeAnimationPlaying && !hasTriggeredDead)
            {
                _animator.SetTrigger("TriggerMerge");
                isMergeAnimationPlaying = true;
            }

            if (isMergeAnimationPlaying && !hasTriggeredDead)
            {
                if (_eyesControl.target != mergingNeighbor.transform)
                {
                    _eyesControl?.SetTargetEyes(mergingNeighbor.transform);
                }
            }

            // Countdown thời gian trigger merge
            timeTriggerMerge -= Time.deltaTime;
        }
        else if (timeTriggerMerge <= 0f && isMergeAnimationPlaying)
        {
            // Kết thúc merge animation, trigger back
            isMergeAnimationPlaying = false;

            _animator.SetTrigger("TriggerIdle");
            _eyesControl?.SetTargetEyes(_eyesControl.target);

            // Reset timer để tránh trigger lại
            timeTriggerMerge = 0f;
        }

        if (GameManager.instance.isGameOver && !hasTriggeredDead)
        {
            _animator.SetTrigger("TriggerDead");
            _rigidbody.bodyType = RigidbodyType2D.Static;
            enabled = false;
        }
        else if (GameManager.instance.isGameOver && hasTriggeredDead)
        {
            _deadEffectSequence?.Kill();
            _rigidbody.bodyType = RigidbodyType2D.Static;
            enabled = false;
        }

        if ((GameManager.MouseState == mouseState.DestroyChoosing || GameManager.MouseState == mouseState.UpgradeChoosing) && _moveCircle.isDrop)
        {
            // Debug.Log($"[{name}] MouseState is DestroyChoosing, enabling AimingComponent.");
            if (aimingGO != null)
                aimingGO.SetActive(true); // Hiện AimingComponent khi đang chọn phá hủy
        }
        else
        {
            // Tắt AimingComponent khi không ở trạng thái DestroyChoosing
            if (aimingGO != null && aimingGO.activeSelf)
                aimingGO.SetActive(false);
        }
    }


    private Sequence _deadEffectSequence;

    private void PlayDeadEffect()
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        var visual = sr != null ? sr.transform : null;
        if (sr == null || visual == null) return;

        sr.color = Color.white;
        _deadEffectSequence?.Kill();

        _deadEffectSequence = DOTween.Sequence().SetLink(gameObject);

        // Đổi màu nhấp nháy
        _deadEffectSequence.Append(sr.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.3f).SetLoops(6, LoopType.Yoyo));

        // Scale in–out đồng thời với màu (tạo tween song song)
        _deadEffectSequence.Join(
            visual.DOScale(visual.localScale * 1.1f, 0.3f).SetLoops(6, LoopType.Yoyo)
        );

        _deadEffectSequence.OnComplete(() =>
        {
            sr.DOColor(new Color(0.5f, 0.5f, 0.5f), 0f);
            visual.localScale = Vector3.one; // Reset scale
            DelayCheckGameOver();
        });
    }

    private void StopDeadEffect()
    {
        _deadEffectSequence?.Kill(false);
        _deadEffectSequence = null;

        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.white;

        var visual = sr != null ? sr.transform : null;
        if (visual != null)
            visual.localScale = Vector3.one;
    }




    public void DelayCheckGameOver()
    {
        if (!isOverLineTriggered && !GameManager.instance.isGameOver)
        {
            GameManager.instance.GameOver();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.GetComponent<MoveCircle>().enabled) return;
        // Va chạm tường bên lần đầu không tính va chạm đầu => tránh trigger dead anim
        if (isFirstCollision && ((collision.GetContact(0).collider.gameObject.name == "LeftWall" ||
            collision.GetContact(0).collider.gameObject.name == "RightWall")))
        {
            // Debug.Log($"{name} va chạm với {collision.GetContact(0).collider.gameObject.name} lần đầu, bỏ qua");
            return;
        }

        // Debug.Log($"{name} va chạm với {collision.gameObject.name}");
        // --- 1. Xử lý squash/stretch nếu có ---
        if (TryGetComponent(out SquashStretch squash))
        {
            float velocity = collision.relativeVelocity.magnitude;
            if (velocity >= squash.minVelocity)
            {
                Vector2 normal = collision.GetContact(0).normal;
                Vector2 contact = collision.GetContact(0).point;
                squash.TriggerSquash(normal, velocity, contact, isFirstCollision);
            }
        }
        if (isFirstCollision)
        {
            isFirstCollision = false;
            // StartCoroutine(DelayCheckGameOver());
        }

        // 1. Nếu đã đang merge → bỏ qua
        if (isMerging) return;

        // 2. Kiểm tra đối tượng va chạm
        if (!collision.gameObject.TryGetComponent(out CircleComponent otherCircle)) return;

        // 3. Nếu cấp khác nhau hoặc đối tượng kia đang merge → bỏ qua
        if (this.Level != otherCircle.Level || otherCircle.isMerging) return;

        // 4. Đảm bảo chỉ 1 trong 2 xử lý
        if (this.GetInstanceID() < otherCircle.GetInstanceID())
        {
            // 5. Đánh dấu đang merge
            isMerging = true;
            otherCircle.isMerging = true;

            // 6. Xác định vị trí merge (lấy điểm tiếp xúc đầu tiên)
            contactPoint = collision.GetContact(0).point;

            // 7. Gọi hàm merge từ GameManager
            OnCircleMerged?.Invoke(this, otherCircle, contactPoint);
        }
    }


    public void RunStretchAnimation(Vector2 normal)
    {
        Vector2 localNormal = transform.worldToLocalMatrix.MultiplyVector(normal.normalized);
        StartCoroutine(StretchAnimation(0.15f, localNormal));
    }

    IEnumerator StretchAnimation(float duration, Vector2 direction)
    {
        stretchMaterial.SetVector("_direction", direction);
        float angle = 180 * Mathf.Deg2Rad;
        float timer = 0;
        while (timer < duration)
        {
            stretchMaterial.SetFloat("_stretch", Mathf.Sin(timer * angle / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private void ChangeToUpgrade()
    {
        int nextLevel = Level + 1;
        if (nextLevel <= evolutionTree.GetMaxLevel() + 1)  //vẫn trong mảng circle có thể next được
        {
            GameObject newObj = GameManager.instance.SpawnAnimalAtLevel(nextLevel, transform.position);
            if (newObj != null)
            {
                PracticeEffect?.Invoke("VFX/Custom_RespawnExplosion", gameObject.transform.position, evolutionTree.levels[nextLevel - 1].colorEffect, nextLevel);
                AudioManager.instance.PlayBoosterUpgradeSound();
                // Gán parent nếu cần
                newObj.transform.SetParent(GameObject.Find("Circles").transform);

                // Vô hiệu hóa LineRenderer nếu có
                LineRenderer lr = newObj.GetComponent<LineRenderer>();
                if (lr != null) lr.enabled = false;

                // Tùy chọn: Tạm tắt điều khiển (nếu cần delay)
                MoveCircle mv = newObj.GetComponent<MoveCircle>();
                if (mv != null) mv.enabled = false;

                // Bật Collider của con sau khi spawn
                Collider2D childCollider = newObj.GetComponentInChildren<CircleCollider2D>();
                if (childCollider != null)
                {
                    childCollider.enabled = true;
                }
                else
                {
                    Debug.LogWarning($"[{newObj.name}] Không tìm thấy CircleCollider2D trong object con.");
                }

                //Check điều kiện Game over
                newObj.GetComponent<CircleComponent>().isOverLineTriggered = true;
            }
            AfterUpgrade?.Invoke(newObj);
        }
        else
        {
            PracticeEffect?.Invoke("VFX/Custom_FruitExplosion", gameObject.transform.position, Color.white, nextLevel);
            AudioManager.instance.PlayBoosterUpgradeSound();
        }

    }

    private void ApplyFixedOutlineWidth()
    {
        if (outlineMaterial == null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            outlineMaterial = sr.material;
        }

        Vector3 scale = transform.lossyScale;
        float avgScale = (scale.x + scale.y) * 0.5f;

        float compensated = GameManager.instance.BaseOutlineWidth / avgScale;

        outlineMaterial.SetFloat("_InnerOutlineWidth", compensated);
    }

}
