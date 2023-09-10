using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class AlertPopupGradientController : MonoBehaviour
{
    [SerializeField] private List<UIGradient> outerGradients;
    [SerializeField] private List<UIGradient> innerGradients;
    [SerializeField] private List<UIGradient> setToOuterAlpha;
    [SerializeField] private List<UIGradient> setToInnerAlpha;
    [SerializeField] private List<UIGradient> gradientBorders;
    [SerializeField] private List<UIGradient> gradientCenters;

    [Separator("Settings")] [SerializeField]
    private Color backgroundColor;

    [SerializeField] private Color outlineColor;
    [SerializeField] private float fadeWidth = 100;
    [SerializeField] private float centerWidth = 100;

    [ButtonMethod]
    public void ApplySettings()
    {
        SetGradientColor(outerGradients, outlineColor);
        SetGradientAlpha(setToOuterAlpha, outlineColor);

        SetGradientColor(innerGradients, backgroundColor);
        SetGradientAlpha(setToInnerAlpha, backgroundColor);

        ApplyFadeWidth();
        ApplyCenterWidth();
    }

    private void ApplyCenterWidth()
    {
        foreach (UIGradient gradientCenter in gradientCenters)
        {
            RectTransform rectTransform = (RectTransform)gradientCenter.transform;

            rectTransform.sizeDelta = new(centerWidth, rectTransform.rect.y);
        }
    }

    private void ApplyFadeWidth()
    {
        foreach (UIGradient gradientBorder in gradientBorders)
        {
            RectTransform rectTransform = (RectTransform)gradientBorder.transform;

            rectTransform.sizeDelta = new(fadeWidth, rectTransform.rect.y);
        }
    }

    private void SetGradientColor(List<UIGradient> gradientList, Color gradientColor)
    {
        foreach (UIGradient gradient in gradientList)
        {
            gradient.m_color1 = gradientColor;

            // keep alpha on the second color
            Color color2 = gradientColor;
            color2.a = gradient.m_color2.a;

            gradient.m_color2 = color2;
        }
    }

    private void SetGradientAlpha(List<UIGradient> gradientList, Color gradientColor)
    {
        foreach (UIGradient gradient in gradientList)
        {
            // set alpha of outline to given gradient comp
            Color innerColor = gradient.m_color2;
            innerColor.a = gradientColor.a;
            gradient.m_color2 = innerColor;
        }
    }
}