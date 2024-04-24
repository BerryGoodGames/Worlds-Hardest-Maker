using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class AnchorAttachable : MonoBehaviour
{
    [InitializationField] [MustBeAssigned] public SpriteRenderer MainSprite;
    [InitializationField] public bool HasSortingGroup;
    [ConditionalField(nameof(HasSortingGroup))] [InitializationField] [CanBeNull] public SortingGroup SortingGroup;
    [FormerlySerializedAs("hasOutline")] [InitializationField] public bool HasOutline;
    [ConditionalField(nameof(HasOutline))] [InitializationField] [MustBeAssigned] public FieldOutline OutlineComp;
}