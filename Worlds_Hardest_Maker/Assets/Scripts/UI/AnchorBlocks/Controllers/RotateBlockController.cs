using MyBox;
using TMPro;

public class RotateBlockController : AnchorBlockController
{
    [Separator("Specifics")] [InitializationField]
    public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(Input.text, out float iterations)) throw new("Input in a Rotate Block was not a float");

        return new RotateBlock(anchorController, IsLocked, iterations);
    }
}