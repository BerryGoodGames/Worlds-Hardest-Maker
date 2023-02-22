using TMPro;
using UnityEngine;

public class WaitBlockController : AnchorBlockController
{
    [SerializeField] private TMP_InputField input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(input.text, out float waitTime)) throw new("Input in a Wait Block was not a float");
        return new WaitBlock(anchorController, waitTime);
    }
}