using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowDrag : MonoBehaviour, IDragHandler
{
    private RectTransform rt;
    private CanvasScaler canvasScaler;

    private Canvas canvas;

    private void Awake()
    {
        rt = transform.parent.GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas = GameManager.Instance.Canvas.GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
