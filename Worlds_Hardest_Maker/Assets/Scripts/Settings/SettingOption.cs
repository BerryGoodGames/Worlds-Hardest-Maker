using TMPro;
using UnityEngine;

public class SettingOption : MonoBehaviour
{
    [SerializeField] protected RectTransform RectTransform;

    public TMP_Text Label;
    [Space] [SerializeField] private float fontSize = 40;

    [SerializeField] public float Height = 80;

    public float FontSize
    {
        get => fontSize;
        set => fontSize = value;
    }

    public void SetHeight(float h)
    {
        Height = h;
        RectTransform.sizeDelta = new(0, h);
    }

    public void UpdateHeight() => SetHeight(Height);

    public void SetFontSize(float size)
    {
        fontSize = size;
        Label.fontSize = size;
    }

    public void UpdateFontSize() => SetFontSize(fontSize);

    public virtual void Response()
    {
        UpdateHeight();
        UpdateFontSize();
    }
}