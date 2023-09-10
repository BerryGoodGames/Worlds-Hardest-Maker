using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockDropdownColor : AnchorBlockColorController
{
    [Space] [SerializeField] private Image imageComp;
    [SerializeField] private Image template;
    [SerializeField] private Toggle templateItem;

    public override void UpdateColor()
    {
        Color darker = GetDarkenedColor(Color, Darkening);

        imageComp.color = KeepA(darker, imageComp.color);

        ColorBlock colorsTemplate = templateItem.colors;
        colorsTemplate.normalColor = KeepA(Color, colorsTemplate.normalColor);
        colorsTemplate.highlightedColor = KeepA(darker, colorsTemplate.highlightedColor);
        colorsTemplate.pressedColor = KeepA(Color, colorsTemplate.pressedColor);
        colorsTemplate.selectedColor = KeepA(darker, colorsTemplate.selectedColor);
        colorsTemplate.disabledColor = KeepA(Color, colorsTemplate.disabledColor);
        templateItem.colors = colorsTemplate;

        template.color = KeepA(Color, template.color);

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