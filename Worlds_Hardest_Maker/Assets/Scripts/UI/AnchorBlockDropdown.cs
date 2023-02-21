using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockDropdown : AnchorBlockColorController
{
    [Space]
    [SerializeField] private Image template;
    [SerializeField] private Toggle templateItem;

    public override void UpdateColor()
    {
        Color darker = GetDarkColor(color, darkening);

        ColorBlock colors = templateItem.colors;
        colors.normalColor = KeepA(color, colors.normalColor);
        colors.highlightedColor = KeepA(color, colors.highlightedColor);
        colors.pressedColor = KeepA(darker, colors.pressedColor);
        colors.selectedColor = KeepA(color, colors.selectedColor);
        colors.disabledColor = KeepA(darker, colors.disabledColor);

        templateItem.colors = colors;

        template.color = KeepA(color, template.color);
    }
}
