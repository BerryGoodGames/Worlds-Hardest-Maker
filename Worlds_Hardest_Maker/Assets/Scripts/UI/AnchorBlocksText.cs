using UnityEngine;
using UnityEngine.UI;

public class AnchorBlocksText : MonoBehaviour
{
    [SerializeField] private RectTransform text;
    [SerializeField] private AspectRatioFitter arfComp;
    [SerializeField] private LayoutElement layoutComp;

    public void UpdateLayoutWidth()
    {
        // Calculate the resulting width of the Aspect Ratio Fitter component
        float resultingWidth = arfComp.aspectRatio * text.rect.height;

        // Set the resulting width as the preferred width of the Layout Element component
        layoutComp.preferredWidth = resultingWidth;
    }
}
