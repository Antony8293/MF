
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class WarningLine : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOverLine;

    [SerializeField]
    private GameObject GameWarningLine;

    private float time;
    private float TargetTime = 1.0f;

    public static bool onWarning = false;
    private SpriteRenderer overLineRenderer;
    [SerializeField]
    private GameObject dynamicBox; // Reference to the dynamic box UI element

    
    private void Start()
    {
        if (GameOverLine != null)
        {
            overLineRenderer = GameOverLine.GetComponent<SpriteRenderer>();
        }

        // Position warning line in upper 1/3 of dynamic box
        PositionWarningLineInDynamicBox();
    }
    private void PositionWarningLineInDynamicBox()
    {
        if (dynamicBox != null && GameWarningLine != null)
        {
            RectTransform boxRect = dynamicBox.GetComponent<RectTransform>();
            BoxCollider2D boxCollider = GameWarningLine.GetComponent<BoxCollider2D>();
            if (boxRect != null && boxCollider != null)
            {
                // Get world corners of the dynamic box
                Vector3[] corners = new Vector3[4];
                boxRect.GetWorldCorners(corners);

                // Calculate width and height in world units
                float width = Vector3.Distance(corners[0], corners[1]); // bottom-left to bottom-right
                float height = Vector3.Distance(corners[0], corners[3]); // bottom-left to top-left

                // Calculate center position for upper 1/3
                Vector3 topCenter = (corners[1] + corners[2]) / 2f;
                Vector3 bottomCenter = (corners[0] + corners[3]) / 2f;
                Vector3 regionCenter = Vector3.Lerp(topCenter, bottomCenter, 1f / 6f); // 1/6 from top = center of upper 1/3

                // Set collider size and position
                boxCollider.size = new Vector2(width, height / 3f);
                GameWarningLine.transform.position = regionCenter;
                boxCollider.offset = Vector2.zero;
                GameWarningLine.transform.rotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("dynamicBox hoặc GameWarningLine thiếu RectTransform hoặc BoxCollider2D!");
            }
        }
        else
        {
            Debug.LogWarning("dynamicBox hoặc GameWarningLine chưa được gán!");
        }
    }

    public void CheckWarning()
    {
        if (!onWarning)
        {
            overLineRenderer.enabled = false;
            time = 0.0f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var parentCircle = collision.transform.parent?.GetComponent<CircleComponent>();
        if (parentCircle != null)
        {
            // Remove CircleComponent khỏi mảng nếu có
            if (GameManager.instance.warningCircles.Contains(parentCircle))
            {
                GameManager.instance.warningCircles.Remove(parentCircle);
            }
        }

        onWarning = false;
        Invoke("CheckWarning", 2f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var parentCircle = collision.transform.parent?.GetComponent<CircleComponent>();
        if (parentCircle != null)
        {
            // Thêm CircleComponent vào mảng nếu chưa có
            if (!GameManager.instance.warningCircles.Contains(parentCircle))
            {
                GameManager.instance.warningCircles.Add(parentCircle);
            }
        }

        onWarning = true;
        //Debug.Log("Enter Warning: " + collision.name + " at " + Time.time);
    }

    private void Update()
    {
        if (onWarning)
        {
            time += math.clamp(Time.deltaTime, 0, TargetTime);
            if (time >= TargetTime && !GameManager.instance.isPaused)
            {
                overLineRenderer.enabled = true;
                // GameOverLine.SetActive(true);
                // Nhấp nháy đỏ
                if (overLineRenderer != null)
                {
                    // Tăng scale X lên 2 lần (hoặc giá trị bạn muốn)
                    overLineRenderer.transform.localScale = new Vector3(4.5f, 0.08f, overLineRenderer.transform.localScale.z);
                    // Fade in nhấp nháy
                    float targetAlpha = Mathf.PingPong(Time.time * 0.5f, 1f); // 0~1, nhấp nháy chậm hơn
                    overLineRenderer.DOFade(targetAlpha, 0.2f).SetUpdate(true).SetEase(Ease.Linear);

                }
            }
        }
    }

    
}
