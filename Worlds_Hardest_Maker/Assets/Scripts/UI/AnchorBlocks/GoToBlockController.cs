using TMPro;
using UnityEngine.Serialization;

public class GoToBlockController : AnchorBlockController
{
    [FormerlySerializedAs("input")] public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!int.TryParse(Input.text, out int index)) throw new("Input in a GoTo Block was not an int");

        return new GoToBlock(anchorController, index);
    }
}