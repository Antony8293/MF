using UnityEngine;

/// <summary>
/// Tự động tạo 3 collider vật lý (trái, phải, dưới) bao bên ngoài UI box, chỉ chạy 1 lần khi Start.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class AutoCreateUIColliderWalls : MonoBehaviour
{
    [Header("Target UI Box")]
    public RectTransform uiTarget;

    [Header("Wall Settings")]
    public float thickness = 1f;
    public string wallTag = "Wall";
    private string wallLayer = "Wall";

    [SerializeField]
    private PhysicsMaterial2D wallMaterial; // Gán từ Inspector nếu cần

    void Start()
    {
        if (uiTarget == null)
        {
            Debug.LogError("AutoCreateUIColliderWalls: RectTransform target chưa được gán.");
            return;
        }

        // Cấu hình Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.simulated = true;

        // Lấy tọa độ 4 góc (world space) của UI box
        Vector3[] corners = new Vector3[4];
        uiTarget.GetWorldCorners(corners);

        Vector2 bottomLeft = corners[0];
        Vector2 topRight = corners[2];
        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        float centerY = bottomLeft.y + height / 2;

        // OUTER walls — dịch ra ngoài
        CreateCollider("LeftWall", new Vector2(thickness, height + thickness * 2), new Vector2(bottomLeft.x - thickness / 2f, centerY));
        CreateCollider("RightWall", new Vector2(thickness, height + thickness * 2), new Vector2(topRight.x + thickness / 2f, centerY));
        CreateCollider("BottomWall", new Vector2(width + thickness * 2, thickness), new Vector2(bottomLeft.x + width / 2f, bottomLeft.y - thickness / 2f));
    }

    void CreateCollider(string name, Vector2 size, Vector2 position)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(this.transform, false);
        wall.transform.position = position;
        wall.transform.rotation = Quaternion.identity;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();

        // Tăng thêm độ dày nếu cần chống xuyên
        col.size = size + new Vector2(0.05f, 0.05f); // chống lọt khe nhỏ

        col.isTrigger = false;

        if (wallMaterial != null)
            col.sharedMaterial = wallMaterial;

        // Kiểm tra layer
        int layerIndex = LayerMask.NameToLayer(wallLayer);
// <<<<<<< boosters
//         if (layerIndex == -1)
//         {
//             Debug.LogWarning($"Layer '{wallLayer}' chưa tồn tại. Vui lòng tạo trong Tags and Layers.");
//         }
//         else
//         {
//             wall.layer = layerIndex;
//         }
// =======
        // Debug.Log($"Layer '{wallLayer}' index: {layerIndex}");
        wall.layer = layerIndex;
// >>>>>>> main

        // Kiểm tra tag
        if (name == "BottomWall")
        {
            if (IsTagDefined("BottomWall")) wall.tag = "BottomWall";
        }
        else
        {
            if (IsTagDefined(wallTag)) wall.tag = wallTag;
        }
    }

    bool IsTagDefined(string tagName)
    {
        try { GameObject.FindWithTag(tagName); return true; }
        catch { return false; }
    }
}
