using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class WaitBlockController : AnchorBlockController
{
    [FormerlySerializedAs("input")] [SerializeField] public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(Input.text, out float waitTime)) throw new("Input in a Wait Block was not a float");
        return new WaitBlock(anchorController, waitTime);
    }
}