using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class AnchorBlockColor : AnchorBlockColorController
{
    [Space]
    [SerializeField] private Image foregroundImage;
    public override void UpdateColor()
    {
        // update bigger background color of anchorblock
        Image imageComp = GetComponent<Image>();
        Color darkened = GetDarkColor(color, darkening);
        imageComp.color = KeepA(darkened, imageComp.color);

        // update smaller background color of anchorblock
        foregroundImage.color = KeepA(color, foregroundImage.color);

        // update color of each input
        AnchorBlockInputDecimal[] inputs = GetComponentsInChildren<AnchorBlockInputDecimal>();
        foreach (AnchorBlockInputDecimal input in inputs)
        {
            input.SetColor(color);
            input.SetDarkening(darkening);
            input.UpdateColor();
        }

        // update color of dropdowns
        AnchorBlockDropdown[] dropdowns = GetComponentsInChildren<AnchorBlockDropdown>();
        foreach (AnchorBlockDropdown dropdown in dropdowns)
        {
            dropdown.SetColor(color);
            dropdown.SetDarkening(darkening);
            dropdown.UpdateColor();
        }
    }
}
