using UnityEngine.UI;

public static class ScrollRectExtension
{
    public static float GetValue(this ScrollRect scrollRect)
    {
        return scrollRect.horizontal ?
            scrollRect.horizontalNormalizedPosition :
            scrollRect.verticalNormalizedPosition;
    }

    public static void SetValue(this ScrollRect scrollRect, float value)
    {
        if (scrollRect.horizontal)
        {
            scrollRect.horizontalNormalizedPosition = value;
        }
        else
        {
            scrollRect.verticalNormalizedPosition = value;
        }
    }
}