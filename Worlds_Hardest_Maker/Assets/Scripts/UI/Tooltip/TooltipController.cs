using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AlphaUITween))]
public class TooltipController : MonoBehaviour
{
    [FormerlySerializedAs("text")] public TMP_Text Text;
}