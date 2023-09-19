using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockIndexInputColor : AnchorBlockColorController
{
    [Space] [SerializeField] private Image container;
    [SerializeField] private Image button;
    [SerializeField] private Image arrow;
    [SerializeField] private List<AnchorBlockColorController> colorControllers;

    public override void UpdateColor()
    {
        Color dark = GetDarkenedColor(Color, Darkening);

        // color container
        container.color = KeepA(dark, container.color);

        // color button
        button.color = KeepA(Color, button.color);

        // color arrow
        arrow.color = KeepA(dark, arrow.color);

        // color colorControllers
        foreach (AnchorBlockColorController controller in colorControllers)
        {
            controller.SetColor(Color);
            controller.UpdateColor();
        }
    }
}