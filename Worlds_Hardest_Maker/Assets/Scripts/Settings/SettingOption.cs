using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingOption : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    public TMP_Text label;
    [Space]
    [SerializeField] private float fontSize = 40;
    [SerializeField] private float height = 80;

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
}