using MyBox;
using TMPro;

public class RotateBlockController : AnchorBlockController
{
    [Separator("Specifics")] [InitializationField] public TMP_InputField IterationsInput;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float iterations = IterationsInput.GetFloatInput();

        return new RotateBlock(anchorController, IsLocked, iterations);
    }
}