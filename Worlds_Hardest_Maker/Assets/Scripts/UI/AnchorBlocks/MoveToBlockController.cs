using TMPro;
using UnityEngine;

public class MoveToBlockController : AnchorBlockController
{
    [SerializeField] private TMP_InputField inputX;
    [SerializeField] private TMP_InputField inputY;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(inputX.text, out float x) || !float.TryParse(inputY.text, out float y))
            throw new("Input in a MoveTo Block was not a float");
        return new MoveToBlock(anchorController, x, y);
    }
}