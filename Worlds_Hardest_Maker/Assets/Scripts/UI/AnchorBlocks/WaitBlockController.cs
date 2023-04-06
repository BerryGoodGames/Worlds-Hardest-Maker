using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class WaitBlockController : AnchorBlockController
{
    [FormerlySerializedAs("Input")] public TMP_InputField DurationInput;
    public TMP_Dropdown UnitInput;

    private WaitBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;

        return selectedUnitString switch
        {
            "s" => WaitBlock.Unit.SECONDS,
            "min" => WaitBlock.Unit.MINUTES,
            "h" => WaitBlock.Unit.HOURS,
            "d" => WaitBlock.Unit.DAYS,
            _ => throw new Exception("WaitBlock: Couldn't parse unit")
        };
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(DurationInput.text, out float waitTime)) throw new("Input in a Wait Block was not a float");

        return new WaitBlock(anchorController, waitTime, GetUnit());
    }
}