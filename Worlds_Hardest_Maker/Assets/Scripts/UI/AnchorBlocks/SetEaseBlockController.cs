using DG.Tweening;
using TMPro;
using UnityEngine;

public class SetEaseBlockController : AnchorBlockController
{
    [SerializeField] private TMP_Dropdown input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        string selectedValue = input.options[input.value].text;
        Ease ease = selectedValue switch
        {
            "linear" => Ease.Linear,
            "ease-out" => Ease.OutCubic,
            "ease-in" => Ease.InCubic,
            "ease-in-out" => Ease.InOutCubic,
            _ => throw new("Input in a SetEase Block was not an option")
        };
        return new TweenBlock(anchorController, ease);
    }
}