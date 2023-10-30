using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;

public class WaitBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, WaitBlock.Unit> unitOptions = new()
    {
        { "s", WaitBlock.Unit.Seconds },
        { "min", WaitBlock.Unit.Minutes },
        { "h", WaitBlock.Unit.Hours },
        { "d", WaitBlock.Unit.Days },
    };

    [Separator("Specifics")] [InitializationField] public TMP_InputField DurationInput;

    [InitializationField] public TMP_Dropdown UnitInput;

    private WaitBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float waitTime = DurationInput.GetFloatInput();

        return new WaitBlock(anchorController, IsLocked, waitTime, GetUnit());
    }

    public static string GetOption(WaitBlock.Unit unit) => unitOptions.FirstOrDefault(x => x.Value == unit).Key;
}