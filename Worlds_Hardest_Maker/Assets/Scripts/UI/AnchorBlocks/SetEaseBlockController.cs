using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetEaseBlockController : AnchorBlockController
{
    [FormerlySerializedAs("input")] [SerializeField] public TMP_Dropdown Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        string selectedValue = Input.options[Input.value].text;
        Ease ease = selectedValue switch
        {
            "linear" => Ease.Linear,
            "ease-out" => Ease.OutCubic,
            "ease-in" => Ease.InCubic,
            "ease-in-out" => Ease.InOutCubic,
            _ => throw new("Input in a SetEase Block was not an option")
        };
        return new SetEaseBlock(anchorController, ease);
    }

    public static string GetOption(Ease ease)
    {
        return ease switch
        {
            Ease.Linear => "linear",
            Ease.OutCubic => "ease-out",
            Ease.InCubic => "ease-in",
            Ease.InOutCubic => "ease-in-out",
            _ => throw new("Ease was not an option")
        };
    }
}