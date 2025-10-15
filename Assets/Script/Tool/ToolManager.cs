using UnityEngine;

public enum ToolType
{
    None,
    Crate
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
            mousePos.z = 0f; // ??t z v? 0 ?? ph? h?p v?i v? tr? 2D
            Instantiate(Crate, mousePos, Quaternion.identity);
        }
    }

    public void CretaChoosen()
    {
        tooltype = ToolType.Crate;
    }
}
