using UnityEngine;
using DG.Tweening;

public class AimingComponent : MonoBehaviour
{
    bool isAiming = false;
    Vector3 originalRotation;
    private Sequence rotationSequence;
    
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
        
        isAiming = true;
        OnAiming(); // Bắt đầu hiệu ứng aiming ngay khi khởi tạo
    }

    public void OnDisable()
    {
        isAiming = false;
        if (rotationSequence != null)
        {
            rotationSequence.Kill(); // Dừng sequence cụ thể
        }
        transform.rotation = Quaternion.Euler(originalRotation); // Trả về góc ban đầu khi tắt
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiming)
        {
            OnChose();
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
