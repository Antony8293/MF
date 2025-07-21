using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class WarningLine : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOverLine;

    private float time;
    private float TargetTime = 1.0f;

    private bool onWarning = false;
    private SpriteRenderer overLineRenderer;
    private void Start()
    {
        if (GameOverLine != null)
        {
            overLineRenderer = GameOverLine.GetComponent<SpriteRenderer>();
        }
    }
    public IEnumerator DelayCheckWarning()
    {
        yield return new WaitForSeconds(2f);
        if (!onWarning)
        {
            overLineRenderer.enabled = false;
            time = 0.0f;
            Debug.Log("Exit Warning: " + Time.time);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onWarning = false;
        StartCoroutine(DelayCheckWarning());
       
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        onWarning = true;    
        // Debug.Log("Enter Warning: " + collision.name + " at " + Time.time);
    }

    private void Update()
    {
        if(onWarning)
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
