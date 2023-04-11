using TMPro;
using UnityEngine.UI;

public class MoveAndRotateBlockController : AnchorBlockController
{
    public TMP_InputField InputX;
    public TMP_InputField InputY;
    public TMP_InputField InputIterations;
    public Toggle AdaptRotation;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(InputX.text, out float x) || !float.TryParse(InputY.text, out float y) ||
            !float.TryParse(InputIterations.text, out float iterations))
            throw new("Input in a MoveAndRotate Block was not a float");
        return new MoveAndRotateBlock(anchorController, x, y, iterations, AdaptRotation.isOn);
    }
}