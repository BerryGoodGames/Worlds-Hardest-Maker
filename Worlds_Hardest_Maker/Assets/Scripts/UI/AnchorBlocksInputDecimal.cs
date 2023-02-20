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
        Color darker = AnchorBlockColor.GetDarkColor(color, darkening);

        imageComp.color = AnchorBlockColor.KeepA(darker, imageComp.color);

        // ColorBlock colors = inputComp.colors;
        // colors.pressedColor = AnchorBlockColor.KeepA(darker, colors.pressedColor);
        // colors.selectedColor = AnchorBlockColor.KeepA(color, colors.selectedColor);
        // colors.disabledColor = AnchorBlockColor.KeepA(color, colors.disabledColor);

        // inputComp.colors = colors;

        inputComp.selectionColor = AnchorBlockColor.KeepA(color, inputComp.selectionColor);
    }

    private static Color KeepA(Color _new, Color assign) => new(_new.r, _new.g, _new.b, assign.a);

    public void SetColor(Color c) => color = c;
    public void SetDarkening(float d) => darkening = d;
}
