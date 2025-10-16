using UnityEngine;

public enum ToolType
{
    None,
    Crate,
    Destroy,
}


public class ToolManager : MonoBehaviour
{

    public GameObject Crate;

    public static ToolType tooltype = ToolType.None;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ToolManager.tooltype == ToolType.Crate)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(mousePos.y > 1.5f || mousePos.x < -1.7f || mousePos.x > 1.7f || mousePos.y < -1.8f) return;
            mousePos.z = 0f; // ??t z v? 0 ?? ph? h?p v?i v? tr? 2D
            Instantiate(Crate, mousePos, Quaternion.identity);
        }
    }

    public void CretaChoosen() => tooltype = ToolType.Crate;

    public void DestroyChoosen() => tooltype = ToolType.Destroy;
}
