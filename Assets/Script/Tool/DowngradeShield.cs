using UnityEngine.UI;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class DowngradeShield : MonoBehaviour
{
    [SerializeField]
    private GameObject Shield2;

    private UnityEngine.UI.Image image;

    void Start()
    {
        image = gameObject.GetComponent<UnityEngine.UI.Image>();
    }

    public void Downgrade()
    {
        if (gameObject.name == "Shield3")
        {
            image.color = Color.yellow;
            gameObject.name = "Shield2";
        }
        else if (gameObject.name == "Shield2")
        {
            image.color = Color.red;
            gameObject.name = "Shield1";
        }
    }
}
