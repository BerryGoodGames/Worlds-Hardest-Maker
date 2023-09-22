using MyBox;
using TMPro;
using UnityEngine.Serialization;

public class RotateBlockController : AnchorBlockController
{
    [FormerlySerializedAs("Input")] [Separator("Specifics")] [InitializationField]
    public TMP_InputField IterationsInput;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float iterations = IterationsInput.GetFloatInput();

        return new RotateBlock(anchorController, IsLocked, iterations);
    }
}