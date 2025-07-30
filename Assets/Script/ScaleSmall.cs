using DG.Tweening;
using UnityEngine;

public class ScaleSmall : MonoBehaviour
{
    private Vector3 orginScale;
    [SerializeField]
    private float duration = 0.5f;  
    private void OnEnable()
    {
        // Booster.booster4 += TriggerBooster;
    }

    private void OnDisable()
    {
        // Booster.booster4 -= TriggerBooster;
    }

    private void TriggerBooster()
    {
        // Lưu lại scale gốc của đối tượng khi kích hoạt
        orginScale = gameObject.transform.localScale;
        // Thực hiện scale xuống 75% kích thước gốc
        transform.DOScale(orginScale * 0.9f, 0);
        Invoke("BackToOrigin", duration);
    }

    private void BackToOrigin()
    {
        transform.DOScale(orginScale, 1.0f);
    }
}
