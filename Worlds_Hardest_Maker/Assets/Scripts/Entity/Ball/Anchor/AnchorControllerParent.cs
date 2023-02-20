using UnityEngine;
using UnityEngine.Serialization;

public class AnchorControllerParent : MonoBehaviour
{
    [FormerlySerializedAs("child")] public AnchorController Child;
}