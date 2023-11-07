using MyBox;
using TMPro;
using UnityEngine.UI;

public class MoveAndRotateBlockController : PositionAnchorBlockController
{
    [Separator("Specifics")] [InitializationField] public TMP_InputField IterationsInput;

    [InitializationField] public Toggle AdaptRotation;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float iterations = IterationsInput.GetFloatInput();

        return new MoveAndRotateBlock(
            anchorController, IsLocked, GetPositionInput(),
            iterations, AdaptRotation.isOn
        );
    }
}