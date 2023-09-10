using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveAndRotateBlockController : PositionAnchorBlockController
{
    [Separator("Specifics")] [InitializationField]
    public TMP_InputField InputIterations;

    [InitializationField] public Toggle AdaptRotation;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(PositionInput.InputX.text, out float x) |
            !float.TryParse(PositionInput.InputY.text, out float y) |
            !float.TryParse(InputIterations.text, out float iterations))
            Debug.LogWarning("Input in a MoveAndRotate Block was not a float");
        return new MoveAndRotateBlock(anchorController, IsLocked, new Vector2(x, y) + anchorController.GetPosition(), iterations, AdaptRotation.isOn);
    }
}