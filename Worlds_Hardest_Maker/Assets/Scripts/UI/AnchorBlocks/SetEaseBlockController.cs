using DG.Tweening;
using TMPro;
using UnityEngine;

public class SetEaseBlockController : AnchorBlockController
{
    [SerializeField] private TMP_InputField input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        Ease ease = input.text switch
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