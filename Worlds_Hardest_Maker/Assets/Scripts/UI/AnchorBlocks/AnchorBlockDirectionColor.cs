using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockDirectionColor : AnchorBlockColorController
{
    [SerializeField] [InitializationField] [Required] private Image backgroundImage;
    
    public override void UpdateColor()
    {
        Color dark = GetDarkenedColor(Color, DARKENING);
        
        backgroundImage.color = KeepA(dark, backgroundImage.color);
    }
}