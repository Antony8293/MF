using DG.Tweening;
using UnityEngine;

public class ScaleSmall : MonoBehaviour
{
     [SerializeField]    
    private float duration = 3f;
    private Vector3 orginScale;

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
        orginScale = gameObject.transform.localScale;

        transform.DOScale(orginScale * 0.75f, duration);
        Invoke("BackToOrigin", duration);
    }

    private void BackToOrigin()
    {
        transform.DOScale(orginScale, duration);
    }
}
