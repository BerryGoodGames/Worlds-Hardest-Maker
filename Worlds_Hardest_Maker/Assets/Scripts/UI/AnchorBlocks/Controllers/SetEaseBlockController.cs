using DG.Tweening;
using MyBox;
using TMPro;

public class SetEaseBlockController : AnchorBlockController
{
    public static readonly UnitDropdownMap<Ease> EaseOptions = new()
    {
        { "linear", Ease.Linear },
        { "ease-out", Ease.OutCubic },
        { "ease-in", Ease.InCubic },
        { "ease-in-out", Ease.InOutCubic },
    };
    
    [Separator("Specifics")] [InitializationField] public TMP_Dropdown Input;
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        string selectedValue = Input.options[Input.value].text;
        Ease ease = EaseOptions[selectedValue];
        return new SetEaseBlock(IsLocked, ease);
    }
}