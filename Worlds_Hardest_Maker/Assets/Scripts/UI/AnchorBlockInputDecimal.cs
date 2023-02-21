using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockInputDecimal : AnchorBlockColorController
{
    [Space]
    [SerializeField] private Image imageComp;
    [SerializeField] private TMP_InputField inputComp;

    public override void UpdateColor()
    {
        Color darker = GetDarkColor(color, darkening);

        imageComp.color = KeepA(darker, imageComp.color);

        inputComp.selectionColor = KeepA(color, inputComp.selectionColor);
    }
}
