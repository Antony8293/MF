using UnityEngine;

public class UIManager : MonoBehaviour
{
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
}
