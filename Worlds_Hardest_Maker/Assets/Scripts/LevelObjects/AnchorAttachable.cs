using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class AnchorAttachable : MonoBehaviour
{
    [InitializationField] [MustBeAssigned] public SpriteRenderer MainSprite;
    [FormerlySerializedAs("hasOutline")] [InitializationField] public bool HasOutline;
    [ConditionalField(nameof(HasOutline))] [InitializationField] [MustBeAssigned] public FieldOutline OutlineComp;
}
