using UnityEngine;

public class BreathingButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isClicked = false; // Biến để theo dõi trạng thái nút
    Vector3 originalScale; // Biến để lưu trữ kích thước gốc của nút
    float scaleSpeed = 5f; // Tốc độ scale
    float scaleAmount = 0.02f; // Mức độ scale (2%)

    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        ScaleTweenLoop();
    }

    void ScaleTweenLoop()
    {
        if (!isClicked)
        {
            // Tạo hiệu ứng breathing bằng sin wave
            float scaleMultiplier = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
            transform.localScale = originalScale * scaleMultiplier;
        }
        else
        {
            // Khi đã click, trở về scale gốc
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);
        }
    }
}
