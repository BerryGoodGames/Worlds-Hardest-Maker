using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetAngularSpeedBlockController : AnchorBlockController
{
    private static Dictionary<string, SetAngularSpeedBlock.Unit> unitOptions = new()
    {
        { "deg / s", SetAngularSpeedBlock.Unit.DEGREES },
        { "it / s", SetAngularSpeedBlock.Unit.ITERATIONS },
        { "s", SetAngularSpeedBlock.Unit.TIME }
    };

    [FormerlySerializedAs("Input")] public TMP_InputField SpeedInput;
    public TMP_Dropdown UnitInput;

    private SetAngularSpeedBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(SpeedInput.text, out float speed)) throw new("Input in a SetAngularSpeed Block was not a float");

        return new SetAngularSpeedBlock(anchorController, speed, GetUnit());
    }

    public static string GetOption(SetAngularSpeedBlock.Unit unit)
    {
        return unitOptions.FirstOrDefault(x => x.Value == unit).Key;
    }
}