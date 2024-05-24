using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockInputDecimalColor : AnchorBlockColorController
{
    [Space] [SerializeField] private Image imageComp;
    [SerializeField] private TMP_InputField inputComp;
    
    public override void UpdateColor()
    {
        Color darker = GetDarkenedColor(Color, DARKENING);
        
        imageComp.color = KeepA(darker, imageComp.color);
        
        inputComp.selectionColor = KeepA(Color, inputComp.selectionColor);
    }
}