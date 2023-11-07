using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockDirectionColor : AnchorBlockColorController
{
    [SerializeField] [InitializationField] [MustBeAssigned] private Image backgroundImage;

    public override void UpdateColor()
    {
        Color dark = GetDarkenedColor(Color, Darkening);

        backgroundImage.color = KeepA(dark, backgroundImage.color);
    }
}