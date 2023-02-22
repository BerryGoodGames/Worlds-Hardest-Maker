using TMPro;
using UnityEngine;

public class GoToBlockController : AnchorBlockController
{
    [SerializeField] private TMP_InputField input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!int.TryParse(input.text, out int index)) throw new("Input in a GoTo Block was not an int");

        return new GoToBlock(anchorController, index);
    }
}