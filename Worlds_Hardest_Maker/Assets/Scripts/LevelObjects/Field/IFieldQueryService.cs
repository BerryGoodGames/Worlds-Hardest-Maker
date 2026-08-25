using JetBrains.Annotations;
using UnityEngine;

public interface IFieldQueryService
{
    public bool IsPosCoveredWithFieldTypeInSheet(Vector2 position, [CanBeNull] AnchorController sheet,
        FieldMode[] allowedFieldTypes);
}