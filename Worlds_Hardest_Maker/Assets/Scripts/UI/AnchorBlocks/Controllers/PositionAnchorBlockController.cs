using MyBox;
using UnityEngine;

public abstract class PositionAnchorBlockController : AnchorBlockController
{
    [Separator("Position")] [InitializationField] [MustBeAssigned]
    public AnchorBlockPositionInputController PositionInput;

    protected Vector2 GetPositionInput()
    {
        if (!float.TryParse(PositionInput.InputX.text, out float x) |
            !float.TryParse(PositionInput.InputY.text, out float y))
            Debug.LogWarning("Input in a Move Block was not a float");
        return new Vector2(x, y);
    }
}