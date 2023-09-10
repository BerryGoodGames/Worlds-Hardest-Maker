using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockColor : AnchorBlockColorController
{
    [Space] [SerializeField] private Image foregroundImage;

    public override void UpdateColor()
    {
        // update bigger background color of anchorblock
        Image imageComp = GetComponent<Image>();
        Color darkened = GetDarkenedColor(Color, Darkening);
        imageComp.color = KeepA(darkened, imageComp.color);

        // update smaller background color of anchorblock
        foregroundImage.color = KeepA(Color, foregroundImage.color);

        // update color of each part of anchor block
        List<AnchorBlockColorController> colorControllers = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out AnchorBlockColorController colorController))
                colorControllers.Add(colorController);
        }

        foreach (AnchorBlockColorController colorController in colorControllers)
        {
            colorController.SetColor(Color);
            colorController.UpdateColor();
        }
    }
}