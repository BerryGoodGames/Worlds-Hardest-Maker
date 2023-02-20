using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlocksInputDecimal : MonoBehaviour
{
    [SerializeField] private Image imageComp;
    [SerializeField] private TMP_InputField inputComp;
    [Space]
    [SerializeField] private Color color;
    [Range(0, 1)][SerializeField] private float darkening;

    public void UpdateColor()
    {
        float percent = 1 - darkening;
        Color darker = new(color.r * percent, color.g * percent, color.b * percent);

        imageComp.color = KeepA(color, imageComp.color);

        ColorBlock colors = inputComp.colors;
        colors.pressedColor = KeepA(darker, colors.pressedColor);
        colors.selectedColor = KeepA(color, colors.selectedColor);
        colors.disabledColor = KeepA(color, colors.disabledColor);

        inputComp.colors = colors;

        inputComp.selectionColor = KeepA(color, inputComp.selectionColor);
    }

    private static Color KeepA(Color _new, Color assign) => new Color(_new.r, _new.g, _new.b, assign.a);
}
