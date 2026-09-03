using MyBox;
using TMPro;
using VContainer;

public class SetRotationBlockController : AnchorBlockController
{
    public static readonly UnitDropdownMap<RotationUnit> UnitOptions = new()
    {
        { "deg / s", RotationUnit.Degrees },
        { "it / s", RotationUnit.Iterations },
        { "s", RotationUnit.Time },
    };
    
    [Separator("Specifics")] [InitializationField] [AutoProperty] public TMP_InputField SpeedInput;
    
    [InitializationField] public TMP_Dropdown UnitInput;

    // TODO: check if injected
    // [Inject] private IAnchorManager anchorManager;
    
    private RotationUnit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return UnitOptions[selectedUnitString];
    }
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new SetRotationBlock(IsLocked, SpeedInput.GetFloatInput(), GetUnit());
    }
    
    public void UpdateWarnings()
    {
        AnchorManager.UpdateBlockListInSelectedAnchor();
        AnchorManager.CheckStartRotatingWarnings();
    }
}