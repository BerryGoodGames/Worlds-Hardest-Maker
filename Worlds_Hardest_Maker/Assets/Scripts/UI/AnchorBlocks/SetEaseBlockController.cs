using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetEaseBlockController : AnchorBlockController
{
    private static Dictionary<string, Ease> easeOptions = new()
    {
        { "linear", Ease.Linear },
        { "ease-out", Ease.OutCubic },
        { "ease-in", Ease.InCubic },
        { "ease-in-out", Ease.InOutCubic }
    };

    [FormerlySerializedAs("input")] [SerializeField] public TMP_Dropdown Input;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        string selectedValue = Input.options[Input.value].text;
        Ease ease = easeOptions[selectedValue];
        return new SetEaseBlock(anchorController, ease);
    }

    public static string GetOption(Ease ease)
    {
        return easeOptions.FirstOrDefault(x => x.Value == ease).Key;
    }
}