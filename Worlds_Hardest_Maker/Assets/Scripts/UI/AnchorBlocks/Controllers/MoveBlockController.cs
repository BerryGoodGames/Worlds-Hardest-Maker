using UnityEngine;

public class MoveBlockController : PositionAnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(PositionInput.InputX.text, out float x) |
            !float.TryParse(PositionInput.InputY.text, out float y))
            Debug.LogWarning("Input in a Move Block was not a float");
        return new MoveBlock(anchorController, IsLocked, x, y);
    }
}