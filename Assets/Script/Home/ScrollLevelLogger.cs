using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLevelLogger : MonoBehaviour
{
    public ScrollRect scrollRect;  // Kéo ScrollRect vào đây trong Inspector
    public int totalLevels = 10;   // Tổng số level hiển thị (hoặc số item con trong Content)

    public TextMeshProUGUI levelText;

    private void Start()
    {
        // Đăng ký sự kiện OnValueChanged
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void OnScrollChanged(Vector2 value)
    {
        // value.y từ 0 -> 1 (0 = đáy, 1 = đỉnh)
        // Chuyển giá trị cuộn thành chỉ số Level
        // Khi cuộn ở đỉnh -> level đầu tiên, cuộn hết -> level cuối

        int currentLevel = Mathf.RoundToInt(value.y * (totalLevels - 1)) + 1;

        Debug.Log($"📜 Đang ở Level: {currentLevel}");

        levelText.text = $"Level: {currentLevel}";
    }
}
