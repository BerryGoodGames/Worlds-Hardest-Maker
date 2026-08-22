using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;

public class SetSpeedBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, SetSpeedBlock.Unit> unitOptions = new()
    {
        { "m / s", SetSpeedBlock.Unit.UnitsPerSecond },
        { "s", SetSpeedBlock.Unit.SecondsToFinish },
    };
    
    [Separator("Specifics")] [InitializationField] public TMP_InputField SpeedInput;
    
    [InitializationField] public TMP_Dropdown UnitInput;
    
    private SetSpeedBlock.Unit GetUnit()
    {
        string selectedUnitString = UnitInput.options[UnitInput.value].text;
        return unitOptions[selectedUnitString];
    }
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        float time = SpeedInput.GetFloatInput();
        
        return new SetSpeedBlock(IsLocked, time, GetUnit());
    }
    
    public static string GetOption(SetSpeedBlock.Unit unit) => unitOptions.FirstOrDefault(x => x.Value == unit).Key;
}