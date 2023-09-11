using MyBox;
using TMPro;
using UnityEngine;

public class LoopBlockController : AnchorBlockController
{
    // [Separator("Specifics")] [InitializationField]
    // public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        // if (!int.TryParse(Input.text, out int index)) Debug.LogWarning("Input in a GoTo Block was not an int");

        return new LoopBlock(anchorController, IsLocked);
    }
}