using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject Crate;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ToolManager.tooltype == ToolType.Crate)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // ??t z v? 0 ?? ph? h?p v?i v? tr? 2D
            Instantiate(Crate, mousePos, Quaternion.identity);
        }
    }
}
