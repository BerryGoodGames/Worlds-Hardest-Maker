using TMPro;
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

        ColorBlock colorsTemplate = templateItem.colors;
        colorsTemplate.normalColor = KeepA(color, colorsTemplate.normalColor);
        colorsTemplate.highlightedColor = KeepA(darker, colorsTemplate.highlightedColor);
        colorsTemplate.pressedColor = KeepA(color, colorsTemplate.pressedColor);
        colorsTemplate.selectedColor = KeepA(darker, colorsTemplate.selectedColor);
        colorsTemplate.disabledColor = KeepA(color, colorsTemplate.disabledColor);
        templateItem.colors = colorsTemplate;

        template.color = KeepA(color, template.color);

        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        ColorBlock colorsDropdown = dropdown.colors;
        colorsTemplate.normalColor = KeepA(darker, colorsTemplate.normalColor);
        colorsTemplate.highlightedColor = KeepA(darker, colorsTemplate.highlightedColor);
        colorsTemplate.pressedColor = KeepA(darker, colorsTemplate.pressedColor);
        colorsTemplate.selectedColor = KeepA(darker, colorsTemplate.selectedColor);
        colorsTemplate.disabledColor = KeepA(darker, colorsTemplate.disabledColor);
        dropdown.colors = colorsDropdown;
    }
}
