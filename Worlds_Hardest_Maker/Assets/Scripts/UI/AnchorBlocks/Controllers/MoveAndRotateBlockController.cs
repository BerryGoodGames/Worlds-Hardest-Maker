using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MoveAndRotateBlockController : PositionAnchorBlockController
{
    [FormerlySerializedAs("InputIterations")] [Separator("Specifics")] [InitializationField]
    public TMP_InputField IterationsInput;

    [InitializationField] public Toggle AdaptRotation;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float x = PositionInput.InputX.GetFloatInput();
        float y = PositionInput.InputY.GetFloatInput();
        float iterations = IterationsInput.GetFloatInput();

        return new MoveAndRotateBlock(anchorController, IsLocked, new Vector2(x, y) + anchorController.GetPosition(),
            iterations, AdaptRotation.isOn);
    }
}