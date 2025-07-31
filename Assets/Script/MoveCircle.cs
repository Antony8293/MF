using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class MoveCircle : MonoBehaviour
{
    public static event Action Setup;

    public static event Action SetDropping;

    public static event Action<String, Vector3, Color, int> PracticeEffect;

    public bool isDrop = false;

    private bool isDragging = false;
    public bool isReady = true;

    private float yOffset;
    private Vector3 offset;

    [SerializeField]
    // private GameObject Hook;

    private float GameoverY;

    private float EvolutionTreeY;
    CircleCollider2D childCollider;

    [SerializeField]
    float maxForceToAddWhenDrop = 2f; // Lực tối đa khi thả

    private float leftLimitX;
    private float rightLimitX;
    public bool isBlockByUI = false;
    private bool isBlockedUntilMouseUp = false;

    void Start()
    {
        GameoverY = GameObject.Find("NextCirclePoint").transform.position.y;
        EvolutionTreeY = GameObject.Find("EvolutionTree").transform.position.y;

        childCollider = GetComponentInChildren<CircleCollider2D>();
        childCollider.enabled = false;


        // Lấy vị trí x của LeftWall và RightWall
        var leftWall = GameObject.Find("LeftWall");
        var rightWall = GameObject.Find("RightWall");
        if (leftWall != null && rightWall != null)
        {
            // Lấy cạnh trong cùng của tường (giả sử tường là BoxCollider2D)
            var leftCol = leftWall.GetComponent<BoxCollider2D>();
            var rightCol = rightWall.GetComponent<BoxCollider2D>();
            if (leftCol != null && rightCol != null)
            {
                leftLimitX = leftWall.transform.position.x + leftCol.size.x / 2f;
                rightLimitX = rightWall.transform.position.x - rightCol.size.x / 2f;
            }
            else
            {
                leftLimitX = leftWall.transform.position.x;
                rightLimitX = rightWall.transform.position.x;
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy LeftWall hoặc RightWall!");
            leftLimitX = -100f;
            rightLimitX = 100f;
        }
    }
    private void OnEnable()
    {
        CircleComponent.AfterUpgrade += SetupInstiate;

        // Hook = GameObject.Find("Hook");
    }


    private void OnMouseDown()
    {
        if (GameManager.MouseState == mouseState.DestroyChoosing && isDrop)
        {
            AudioManager.instance.PlayBoosterHammerSound(); // Phát âm thanh khi nhấn nút
            FinishBosster(true);
        }
        else if (GameManager.MouseState == mouseState.UpgradeChoosing && isDrop)
        {
            gameObject.GetComponent<CircleComponent>()?.OnUpgrade?.Invoke();

            FinishBosster(false);

        }
    }

    void Update()
    {
        if (isBlockByUI)
        {
            // Khi đang block, nếu có thả chuột thì gỡ block chờ thả
            if (Input.GetMouseButtonUp(0))
            {
                isBlockedUntilMouseUp = false;
            }
            else
            {
                isBlockedUntilMouseUp = true;
            }

            return;
        }

        if (isBlockedUntilMouseUp)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isBlockedUntilMouseUp = false;
            }
            return;
        }
        // Gộp bắt đầu kéo và kéo thành 1
        if (Input.GetMouseButton(0) && !isDrop && GameManager.MouseState == mouseState.notChoosing && isReady)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Kiểm tra giới hạn Y
            if (mousePos.y > GameoverY || mousePos.y < EvolutionTreeY) return;

            isDragging = true;

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
            }

            if (childCollider != null)
            {
                float radius = childCollider.radius * childCollider.transform.lossyScale.x;

                // Chỉ clamp và di chuyển theo trục x, giữ nguyên y
                float clampedX = Mathf.Clamp(mousePos.x, leftLimitX + radius, rightLimitX - radius);
                Vector3 clamped = new Vector3(clampedX, transform.position.y, 0);

                if (rb != null)
                {
                    rb.MovePosition(clamped);
                }
            }
            else
            {
                Debug.LogWarning($"[{name}] Không tìm thấy CircleCollider2D trong object con.");
            }
        }

        // 2. Thả vật thể
        if (Input.GetMouseButtonUp(0) && isDragging && !isDrop)
        {
            gameObject.GetComponent<LineRenderer>().enabled = false;
            isDragging = false;
            isDrop = true;

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 2;

                float force = maxForceToAddWhenDrop * rb.mass;
                // rb.AddForce(Vector2.down * force, ForceMode2D.Impulse);
            }

            transform.SetParent(GameObject.Find("Circles").transform);

            Setup?.Invoke();
            SetDropping?.Invoke();

            childCollider.enabled = true;
            GameManager.instance.droppingCirclePos = transform.position;
            childCollider.gameObject.layer = LayerMask.NameToLayer("Default");

            enabled = false; // tắt MoveCircle

            if (GameManager.instance.isPlayingTutorial)
            {
                GameManager.instance.isPlayingTutorial = false;
                TutorialManager.instance.CloseTutorial();
            }
        }

        // Game Over
        if (GameManager.instance.isGameOver)
        {
            Destroy(gameObject);
        }

    }



    private void SetupInstiate(UnityEngine.Object circle)
    {
        circle.GetComponent<MoveCircle>().isDrop = true;
        circle.GetComponent<MoveCircle>().isDragging = false;
    }

    private void FinishBosster(Boolean isColorEffect = false)
    {
        if (isColorEffect)
        {
            PracticeEffect?.Invoke("VFX/Custom_FruitExplosion", gameObject.transform.position, gameObject.GetComponent<CircleComponent>().evolutionTree.levels[gameObject.GetComponent<CircleComponent>().Level - 1].colorEffect, gameObject.GetComponent<CircleComponent>().Level);
        }

        GameManager.TriggerMouseNotChoosing();

        Destroy(gameObject);
    }
}
