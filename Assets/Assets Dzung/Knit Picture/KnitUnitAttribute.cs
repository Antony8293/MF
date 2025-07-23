using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KnitUnitAttribute : MonoBehaviour
{
    private Image _imageKnitUnit;
    void Awake()
    {
        _imageKnitUnit = GetComponent<Image>();
    }
    // public void SetColorKnitUnit(Wool wool)
    // {
    //     _imageKnitUnit.color = ColorManager.Instance.Colors[wool.InxWool];
    // }
    public void ScaleKnitUnit()
    {
        transform.localPosition = Vector3.one;
        Vector3 newScale = new Vector3(1.1f, 1.1f, 1.1f);

        transform.DOScale(newScale, 0.05f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.05f).OnComplete(() => transform.localPosition = Vector3.one);
        });
    }
}
