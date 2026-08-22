using MyBox;
using TMPro;

public class WaitBlockController : AnchorBlockController
{
    public static readonly UnitDropdownMap<TimeUnit> unitOptions = new()
    {
        { "s", TimeUnit.Seconds },
        { "min", TimeUnit.Minutes },
        { "h", TimeUnit.Hours },
        { "d", TimeUnit.Days },
    };
    
    [Separator("Specifics")] [InitializationField] public TMP_InputField DurationInput;
    
    [InitializationField] public TMP_Dropdown UnitInput;
    
    private TimeUnit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float waitTime = DurationInput.GetFloatInput();
        
        return new WaitBlock(IsLocked, waitTime, GetUnit());
    }
}