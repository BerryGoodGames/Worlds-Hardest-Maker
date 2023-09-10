using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;

public class SetSpeedBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, SetSpeedBlock.Unit> unitOptions = new()
    {
        { "m / s", SetSpeedBlock.Unit.Speed },
        { "s", SetSpeedBlock.Unit.Time }
    };

    [Separator("Specifics")] [InitializationField]
    public TMP_InputField SpeedInput;

    [InitializationField] public TMP_Dropdown UnitInput;

    private SetSpeedBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(SpeedInput.text, out float time)) throw new("Input in a SetSpeed Block was not a float");

        return new SetSpeedBlock(anchorController, IsLocked, time, GetUnit());
    }

    public static string GetOption(SetSpeedBlock.Unit unit) => unitOptions.FirstOrDefault(x => x.Value == unit).Key;
}