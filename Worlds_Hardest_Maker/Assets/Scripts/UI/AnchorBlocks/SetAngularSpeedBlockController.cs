using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetAngularSpeedBlockController : AnchorBlockController
{
    [FormerlySerializedAs("Input")] public TMP_InputField SpeedInput;
    public TMP_Dropdown UnitInput;

    private SetAngularSpeedBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;

        return selectedUnitString switch
        {
            "deg / s" => SetAngularSpeedBlock.Unit.DEGREES,
            "it / s" => SetAngularSpeedBlock.Unit.ITERATIONS,
            "s" => SetAngularSpeedBlock.Unit.TIME,
            _ => throw new Exception("SetAngularSpeedBlock: Couldn't parse unit")
        };
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(SpeedInput.text, out float speed)) throw new("Input in a SetAngularSpeed Block was not a float");

        return new SetAngularSpeedBlock(anchorController, speed, GetUnit());
    }
}