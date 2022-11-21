using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingOption : MonoBehaviour
{
    [SerializeField] protected RectTransform rectTransform;
    public TMP_Text label;
    [Space]
    [SerializeField] private float fontSize = 40;
    [SerializeField] protected float height = 80;
    public float FontSize { get => fontSize; set { fontSize = value; } }
    public float Height { get => height; set { height = value; } }

    public void SetHeight(float h)
    {
        height = h;
        rectTransform.sizeDelta = new(0, h);
    }
    public void UpdateHeight()
    {
        SetHeight(height);
    }
    public void SetFontSize(float size)
    {
        fontSize = size;
        label.fontSize = size;
    }
    public void UpdateFontSize()
    {
        SetFontSize(fontSize);
    }

    public virtual void Response()
    {
        UpdateHeight();
        UpdateFontSize();
    }
}