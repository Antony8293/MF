using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(ToolManager.tooltype == ToolType.Destroy)
        {
            Destroy(gameObject);
        }
    }
}
