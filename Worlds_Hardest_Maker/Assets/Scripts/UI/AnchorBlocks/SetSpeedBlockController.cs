using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetSpeedBlockController : AnchorBlockController
{
    [FormerlySerializedAs("input")] [SerializeField] public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(Input.text, out float time)) throw new("Input in a SetSpeed Block was not a float");
        return new SetSpeedBlock(anchorController, time, SetSpeedBlock.Unit.SPEED);
    }
}