using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AlphaUITween))]
public class TooltipController : MonoBehaviour
{
    [FormerlySerializedAs("Text")] public TMP_Text text;
}