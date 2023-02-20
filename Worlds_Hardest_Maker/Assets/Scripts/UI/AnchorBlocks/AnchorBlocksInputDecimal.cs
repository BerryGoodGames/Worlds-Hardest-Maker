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

        inputComp.selectionColor = AnchorBlockColor.KeepA(color, inputComp.selectionColor);
    }

    public void SetColor(Color c) => color = c;
    public void SetDarkening(float d) => darkening = d;
}
