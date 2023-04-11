using TMPro;
using UnityEngine.Serialization;

public class RotateBlockController : AnchorBlockController
{
    [FormerlySerializedAs("input")] public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(Input.text, out float iterations)) throw new("Input in a Rotate Block was not a float");

        return new RotateBlock(anchorController, iterations);
    }
}