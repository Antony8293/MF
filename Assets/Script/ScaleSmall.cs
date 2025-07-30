using DG.Tweening;
using UnityEngine;

public class ScaleSmall : MonoBehaviour
{
    private Vector3 orginScale;

    private void OnEnable()
    {
        Booster.booster4 += TriggerBooster;
    }

    private void OnDisable()
    {
        Booster.booster4 -= TriggerBooster;
    }

    private void TriggerBooster()
    {
        orginScale = gameObject.transform.localScale;

        transform.DOScale(orginScale * 0.75f, 1.0f);
        Invoke("BackToOrigin", 1.5f);
    }

    private void BackToOrigin()
    {
        transform.DOScale(orginScale, 1.0f);
    }
}
