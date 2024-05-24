using JetBrains.Annotations;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class AnchorAttachable : MonoBehaviour
{
    [InitializationField] [Required] public SpriteRenderer MainSprite;
    [InitializationField] public bool HasSortingGroup;
    [ConditionalField(nameof(HasSortingGroup))] [InitializationField] [CanBeNull] public SortingGroup SortingGroup;
    [InitializationField] public bool HasOutline;
    [ConditionalField(nameof(HasOutline))] [InitializationField] [Required] public FieldOutline OutlineComp;
}