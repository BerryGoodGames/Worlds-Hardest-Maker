using TMPro;
using UnityEngine;

public class RotateBlockContainer : AnchorBlockController
{
    [SerializeField] private TMP_InputField input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(input.text, out float iterations)) throw new("Input in a Rotate Block was not a float");

        return new RotateBlock(anchorController, iterations);
    }
}