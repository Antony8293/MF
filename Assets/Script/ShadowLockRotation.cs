using UnityEngine;

public class ShadowFollowCircle : MonoBehaviour
{
    public Transform target;             // Quả (cha)
    public Vector3 offset = new Vector3(0f, -0.1f, 0f); // Offset mong muốn từ quả

    private Quaternion fixedRotation;

    void Start()
    {
        fixedRotation = Quaternion.identity; // Giữ shadow luôn hướng thẳng (nếu muốn)
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Đặt vị trí shadow theo target, bỏ qua xoay
            transform.position = target.position + offset;

            // Giữ nguyên góc xoay, không bị ảnh hưởng bởi target
            transform.rotation = fixedRotation;
        }
    }
}
