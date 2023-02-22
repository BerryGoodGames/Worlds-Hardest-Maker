using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetSpeedBlockController : AnchorBlockController
{
    [SerializeField] private TMP_InputField input;
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(input.text, out float time)) throw new("Input in a SetSpeed Block was not a float");
        return new SetSpeedBlock(anchorController, time, SetSpeedBlock.Unit.SPEED);
    }
}
