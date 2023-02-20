using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SettingOption : MonoBehaviour
{
    [FormerlySerializedAs("rectTransform")] [SerializeField] protected RectTransform RectTransform;
    [FormerlySerializedAs("label")] public TMP_Text Label;
    [Space] [SerializeField] private float fontSize = 40;
    [FormerlySerializedAs("height")] [SerializeField] public float Height = 80;

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

    public void UpdateHeight()
    {
        SetHeight(Height);
    }

    public void SetFontSize(float size)
    {
        fontSize = size;
        Label.fontSize = size;
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