using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;

public class SetRotationBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, RotationUnit> unitOptions = new()
    {
        { "deg / s", RotationUnit.Degrees },
        { "it / s", RotationUnit.Iterations },
        { "s", RotationUnit.Time },
    };
    
    [Separator("Specifics")] [InitializationField] [AutoProperty] public TMP_InputField SpeedInput;
    
    [InitializationField] public TMP_Dropdown UnitInput;
    
    private RotationUnit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) =>
        new SetRotationBlock(anchorController, IsLocked, SpeedInput.GetFloatInput(), GetUnit());
    
    public static string GetOption(RotationUnit unit) => unitOptions.FirstOrDefault(x => x.Value == unit).Key;
    
    public void UpdateWarnings()
    {
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();
        AnchorManager.Instance.CheckStartRotatingWarnings();
    }
}