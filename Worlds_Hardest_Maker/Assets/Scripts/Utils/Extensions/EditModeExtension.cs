/// <summary>
///     Extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static WorldPositionType GetWorldPositionType(this EditMode mode)
    {
        if (mode.Attributes.IsField || mode == EditModeManager.Delete) return WorldPositionType.Matrix;
        return WorldPositionType.Grid;
    }
}