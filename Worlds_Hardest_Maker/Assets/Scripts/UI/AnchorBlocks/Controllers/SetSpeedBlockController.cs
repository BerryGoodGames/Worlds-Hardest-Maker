using MyBox;
using TMPro;

public class SetSpeedBlockController : AnchorBlockController
{
    public static readonly UnitDropdownMap<MovementUnit> UnitOptions = new()
    {
        { "m / s", MovementUnit.UnitsPerSecond },
        { "s", MovementUnit.SecondsToFinish },
    };
    
    [Separator("Specifics")] [InitializationField] public TMP_InputField SpeedInput;
    
    [InitializationField] public TMP_Dropdown UnitInput;
    
    private MovementUnit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return UnitOptions[selectedUnitString];
    }
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float time = SpeedInput.GetFloatInput();
        
        return new SetSpeedBlock(IsLocked, time, GetUnit());
    } 
}