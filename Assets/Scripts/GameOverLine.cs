using System;
using Unity.Mathematics;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    // public static event Action GameOVer;

    // private float time;
    // private float TargetTime = 2.0f;
    // private bool isOn = false;

    public GameObject dynamicBox;
    public GameObject lineGameOver;

    void OnTriggerEnter2D(Collider2D collision)
    {
        var parentCircle = collision.transform.parent?.GetComponent<CircleComponent>();
        if (parentCircle != null)
        {
            parentCircle.isOverLineTriggered = true;
            // Debug.Log(parentCircle.isOverLineTriggered + " parentCircle has crossed the GameOver line.");
        }
    }

    // private void OnTriggerExit2D(Collider2D collision) => isOn = false;
    void Start()
    {
        RectTransform boxRect = dynamicBox.GetComponent<RectTransform>();
        Transform overLineTransform = lineGameOver.transform;


        if (boxRect != null && overLineTransform != null)
        {
            // Lấy vị trí world của cạnh trên dynamicBox (UI)
            Vector3[] corners = new Vector3[4];
            boxRect.GetWorldCorners(corners);
            Vector3 topCenter = (corners[1] + corners[2]) / 2f; // 1: top left, 2: top right

            // Đặt overLine vào cạnh trên, căn giữa
            overLineTransform.position = topCenter;
            overLineTransform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("dynamicBox hoặc lineGameOver không có RectTransform!");
        }

    }

}
