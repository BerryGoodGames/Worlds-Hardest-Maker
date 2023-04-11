using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;

public class WaitBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, WaitBlock.Unit> unitOptions = new()
    {
        { "s", WaitBlock.Unit.SECONDS },
        { "min", WaitBlock.Unit.MINUTES },
        { "h", WaitBlock.Unit.HOURS },
        { "d", WaitBlock.Unit.DAYS }
    };

    [FormerlySerializedAs("Input")] public TMP_InputField DurationInput;
    public TMP_Dropdown UnitInput;

    private WaitBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(DurationInput.text, out float waitTime)) throw new("Input in a Wait Block was not a float");

        return new WaitBlock(anchorController, waitTime, GetUnit());
    }

    public static string GetOption(WaitBlock.Unit unit)
    {
        return unitOptions.FirstOrDefault(x => x.Value == unit).Key;
    }
}