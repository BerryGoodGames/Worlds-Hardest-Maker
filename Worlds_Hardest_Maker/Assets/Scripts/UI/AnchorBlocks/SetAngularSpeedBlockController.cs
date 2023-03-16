using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetAngularSpeedBlockController : AnchorBlockController
{
    [FormerlySerializedAs("input")] [SerializeField] public TMP_InputField Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(Input.text, out float speed)) throw new("Input in a SetAngularSpeed Block was not a float");

        return new SetAngularSpeedBlock(anchorController, speed, SetAngularSpeedBlock.Unit.ITERATIONS);
    }
}