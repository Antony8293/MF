using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLevelLogger : MonoBehaviour
{
    public static Action<int> onLevelChoosen;

    public ScrollRect scrollRect;  // Kéo ScrollRect vào đây trong Inspector
    public int totalLevels = 6;   // Tổng số level hiển thị (hoặc số item con trong Content)

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

        levelText.text = $"Level: {currentLevel}";
    }

    public void ChooseLevel()
    {
        int chosenLevel = Int32.Parse(levelText.text.Replace("Level: ", ""));
        onLevelChoosen?.Invoke(chosenLevel);
    } 
}
