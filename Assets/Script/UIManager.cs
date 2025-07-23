using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Booster Popups")]
    public BoosterPopup[] boosterPopups; // Gán 4 popup ở đây theo thứ tự 0-3

    private BoosterPopup currentPopup;
    public GameObject darkBG; // Gán GameObject DarkBG


    public void OpenRateUsURL()
    {
        Application.OpenURL("https://yourlink.com");
    }
    public void OpenTermsOfUseURL()
    {
        Application.OpenURL("https://zegostudio.com/terms.html");
    }
    public void OpenPrivacyPolicyURL()
    {
        Application.OpenURL("https://zegostudio.com/privacy-policy.html");
    }

    public void OpenBoosterPopup(int index)
    {
        // Ẩn popup cũ nếu có
        if (currentPopup != null)
            currentPopup.Hide();

        // Bảo vệ chỉ số
        if (index < 0 || index >= boosterPopups.Length) return;

        currentPopup = boosterPopups[index];
        currentPopup.Show(index);
        darkBG.SetActive(true); // Hiện nền mờ
    }

    public void CloseCurrentPopup()
    {
        if (currentPopup != null)
        {
            currentPopup.Hide();
            currentPopup = null;
        }

        darkBG.SetActive(false); // Ẩn nền mờ
    }
}
