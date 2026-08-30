using UnityEngine;
using VContainer;

public class PreviewOutlineDataProvider
{
    [Inject] private FieldQueryService fieldQueryService;

    public PreviewOutlineData GetOutlineData(EditMode editMode, Vector2 position, ISheet sheet)
    {
        if (editMode is not FieldMode fieldMode || !fieldMode.HasOutline || fieldMode.Prefab == null)
            return PreviewOutlineData.None;

        FieldController existing = fieldQueryService.Find(position, sheet);
        if (existing != null && existing.FieldMode == fieldMode) return PreviewOutlineData.None;

        if (!fieldMode.Prefab.TryGetComponent(out FieldOutline prefabOutline))
            return PreviewOutlineData.None;

        return new()
        {
            Enabled = true,
            Color = prefabOutline.Color,
            Weight = prefabOutline.Weight,
            ConnectorTags = prefabOutline.GetConnectorTags(),
        };
    }
}