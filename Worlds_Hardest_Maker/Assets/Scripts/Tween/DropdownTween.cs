using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropdownTween : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease expandEase;

    public void TweenExpand()
    {
        RectTransform list = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
        
        list.localScale = new(1, 0);
        list.DOScaleY(1, duration).SetEase(expandEase);
    }
}
