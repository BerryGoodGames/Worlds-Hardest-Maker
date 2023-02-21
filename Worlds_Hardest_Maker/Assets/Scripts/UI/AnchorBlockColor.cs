using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        AnchorBlocksInputDecimal[] inputs = GetComponentsInChildren<AnchorBlocksInputDecimal>();
        foreach (AnchorBlocksInputDecimal input in inputs)
        {
            input.SetColor(color);
            input.SetDarkening(darkening);
            input.UpdateColor();
        }

        // update color of dropdowns
        TMP_Dropdown[] dropdowns = GetComponentsInChildren<TMP_Dropdown>();
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            Image img = dropdown.GetComponent<Image>();
            img.color = KeepA(darkened, img.color);
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
