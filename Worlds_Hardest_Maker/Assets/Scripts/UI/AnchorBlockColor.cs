using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class AnchorBlockColor : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [Space]
    [SerializeField] private Color color;
    [Range(0, 1)][SerializeField] private float darkening;
    public void UpdateColor()
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

    public static Color GetDarkColor(Color color, float darkening)
    {
        float percent = 1 - darkening;
        Color darker = new(color.r * percent, color.g * percent, color.b * percent);
        return darker;
    }
    public static Color KeepA(Color _new, Color assign) => new(_new.r, _new.g, _new.b, assign.a);

}
