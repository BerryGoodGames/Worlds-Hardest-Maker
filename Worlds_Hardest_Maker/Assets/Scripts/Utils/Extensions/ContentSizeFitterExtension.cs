using UnityEngine;
using UnityEngine.UI;

public static class ContentSizeFitterExtension
{
    public static void Recalculate(this ContentSizeFitter contentSizeFitter)
    {
        Transform transform = contentSizeFitter.transform;

        // Recalculate the vertical and horizontal layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}