using TMPro;
using UnityEngine;

public class SetAngularSpeedBlockController : AnchorBlockController
{
    [SerializeField] private TMP_InputField input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(input.text, out float speed)) throw new("Input in a SetAngularSpeed Block was not a float");

        return new SetAngularSpeedBlock(anchorController, speed, SetAngularSpeedBlock.Unit.ITERATIONS);
    }
}