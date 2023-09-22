using System;
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
    [AutoProperty] public TMP_InputField SpeedInput;

    private TMPDecimalInputAdjuster inputAdjuster;

    [InitializationField] public TMP_Dropdown UnitInput;

    private void Awake()
    {
        inputAdjuster = GetComponentInChildren<TMPDecimalInputAdjuster>();
    }

    private SetRotationBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) => new SetRotationBlock(anchorController, IsLocked, SpeedInput.GetFloatInput(), GetUnit());

    public static string GetOption(SetRotationBlock.Unit unit) =>
        unitOptions.FirstOrDefault(x => x.Value == unit).Key;

    public void UpdateWarnings()
    {
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();
        AnchorManager.Instance.CheckStartRotatingWarnings();
    }
}