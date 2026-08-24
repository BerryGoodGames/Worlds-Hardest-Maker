/// <summary>
///     Extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static WorldPositionType GetWorldPositionType(this EditMode mode)
    {
        if (mode is FieldMode || mode == EditModeManager.Delete) return WorldPositionType.Matrix;
        return WorldPositionType.Grid;
    }
}