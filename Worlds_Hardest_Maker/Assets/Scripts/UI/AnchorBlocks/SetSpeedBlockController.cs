using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetSpeedBlockController : AnchorBlockController
{
    [FormerlySerializedAs("Input")] public TMP_InputField SpeedInput;
    public TMP_Dropdown UnitInput;

    private SetSpeedBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;

        return selectedUnitString switch
        {
            "m / s" => SetSpeedBlock.Unit.SPEED,
            "s" => SetSpeedBlock.Unit.TIME,
            _ => throw new Exception("SetSpeedBlock: Couldn't parse unit")
        };
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(SpeedInput.text, out float time)) throw new("Input in a SetSpeed Block was not a float");

        return new SetSpeedBlock(anchorController, time, GetUnit());
    }
}