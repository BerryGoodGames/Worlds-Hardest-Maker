using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockDropdown : MonoBehaviour
{
    [SerializeField] private Image template;
    [SerializeField] private Toggle templateItem;
    [Space]
    [SerializeField] private Color color;
    [Range(0, 1)][SerializeField] private float darkening;

    public void UpdateColor()
    {
        Color darker = AnchorBlockColor.GetDarkColor(color, darkening);

        ColorBlock colors = templateItem.colors;
        colors.normalColor = AnchorBlockColor.KeepA(color, colors.normalColor);
        colors.highlightedColor = AnchorBlockColor.KeepA(color, colors.highlightedColor);
        colors.pressedColor = AnchorBlockColor.KeepA(darker, colors.pressedColor);
        colors.selectedColor = AnchorBlockColor.KeepA(color, colors.selectedColor);
        colors.disabledColor = AnchorBlockColor.KeepA(darker, colors.disabledColor);

        templateItem.colors = colors;

        template.color = AnchorBlockColor.KeepA(color, template.color);
    }


    public void SetColor(Color c) => color = c;
    public void SetDarkening(float d) => darkening = d;
}
