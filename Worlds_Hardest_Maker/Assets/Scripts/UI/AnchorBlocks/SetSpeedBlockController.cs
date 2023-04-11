using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetSpeedBlockController : AnchorBlockController
{
    private static Dictionary<string, SetSpeedBlock.Unit> unitOptions = new()
    {
        { "m / s", SetSpeedBlock.Unit.SPEED },
        { "s", SetSpeedBlock.Unit.TIME }
    };

    [FormerlySerializedAs("Input")] public TMP_InputField SpeedInput;
    public TMP_Dropdown UnitInput;

    private SetSpeedBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(SpeedInput.text, out float time)) throw new("Input in a SetSpeed Block was not a float");

        return new SetSpeedBlock(anchorController, time, GetUnit());
    }

    public static string GetOption(SetSpeedBlock.Unit unit)
    {
        return unitOptions.FirstOrDefault(x => x.Value == unit).Key;
    }
}