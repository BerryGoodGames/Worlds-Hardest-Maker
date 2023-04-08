using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveToBlockController : AnchorBlockController
{
    [FormerlySerializedAs("inputX")] public TMP_InputField InputX;
    [FormerlySerializedAs("inputY")] public TMP_InputField InputY;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(InputX.text, out float x) || !float.TryParse(InputY.text, out float y))
            throw new("Input in a MoveTo Block was not a float");
        return new MoveBlock(anchorController, x, y);
    }
}