using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;

public class SetRotationBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, SetRotationBlock.Unit> unitOptions = new()
    {
        { "deg / s", SetRotationBlock.Unit.Degrees },
        { "it / s", SetRotationBlock.Unit.Iterations },
        { "s", SetRotationBlock.Unit.Time }
    };

    [Separator("Specifics")] [InitializationField]
    public TMP_InputField SpeedInput;

    [InitializationField] public TMP_Dropdown UnitInput;

    private SetRotationBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        if (!float.TryParse(SpeedInput.text, out float speed))
            throw new("Input in a SetAngularSpeed Block was not a float");

        return new SetRotationBlock(anchorController, IsLocked, speed, GetUnit());
    }

    public static string GetOption(SetRotationBlock.Unit unit) =>
        unitOptions.FirstOrDefault(x => x.Value == unit).Key;
}