using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MyBox;
using TMPro;

public class SetEaseBlockController : AnchorBlockController
{
    private static readonly Dictionary<string, Ease> easeOptions = new()
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
        Ease ease = easeOptions[selectedValue];
        return new SetEaseBlock(anchorController, IsLocked, ease);
    }

    public static string GetOption(Ease ease) => easeOptions.FirstOrDefault(x => x.Value == ease).Key;
}