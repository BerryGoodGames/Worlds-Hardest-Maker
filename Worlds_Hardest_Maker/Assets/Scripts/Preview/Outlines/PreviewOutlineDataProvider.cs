// File: Preview/Services/PreviewOutlineDataProvider.cs
/// <summary>
///     Reads outline settings (color, weight, connector tags) straight off the real prefab's
///     FieldOutline component, so previews never carry a second, hand-maintained copy of that data.
/// </summary>
public class PreviewOutlineDataProvider
{
    public PreviewOutlineData GetOutlineData(EditMode editMode)
    {
        if (editMode is not FieldMode fieldMode || !fieldMode.HasOutline || fieldMode.Prefab == null)
            return PreviewOutlineData.None;

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