using UnityEngine;
using DG.Tweening;

public class AimingComponent : MonoBehaviour
{
    public bool isTargeted = false;
    Vector3 originalRotation;
    private Sequence rotationSequence;
    private bool hasProcessedBooster = false; // Flag để tránh loop enable/disable
    
    void Awake()
    {
        // Set rotation về (0, 0, 0) và lưu làm originalRotation
        originalRotation = Vector3.zero;
        transform.rotation = Quaternion.Euler(originalRotation);
    }
    
    void Start()
    {
        // Không cần lưu rotation ở đây nữa
    }
    
    void OnEnable()
    {
        // Force rotation về (0, 0, 0) mỗi khi enable
        transform.rotation = Quaternion.Euler(originalRotation);

        isTargeted = false; // Đánh dấu đã chọn mục tiêu
        hasProcessedBooster = false; // Reset flag khi enable
        OnAiming(); // Bắt đầu hiệu ứng aiming ngay khi khởi tạo
    }

    public void OnDisable()
    {
        isTargeted = false;
        if (rotationSequence != null)
        {
            rotationSequence.Kill(); // Dừng sequence cụ thể
        }
        transform.rotation = Quaternion.Euler(originalRotation); // Trả về góc ban đầu khi tắt
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isBoosterTriggered)
        {
            if (isTargeted)
            {
                Debug.Log("AimingComponent: isTargeted = true");
                OnChose();
            }
            else
            {
                gameObject.SetActive(false); // Tắt gameobject nếu không được chọn
            }
            
        }
    }

    public void OnAiming()
    {
        // Dừng sequence cũ nếu có
        if (rotationSequence != null)
        {
            rotationSequence.Kill();
        }
        
        rotationSequence = DOTween.Sequence();
        rotationSequence.Append(transform.DORotate(new Vector3(0, 0, -120), 0.8f).SetEase(Ease.InOutCubic));
        rotationSequence.Append(transform.DORotate(originalRotation, 0.8f).SetEase(Ease.InOutCubic));
        rotationSequence.SetLoops(-1, LoopType.Restart); // Loop vô hạn
    }

    public void OnChose()
    {
        GetComponent<SpriteRenderer>().color = Color.white; // Thay đổi màu sắc khi chọn
    }

}
