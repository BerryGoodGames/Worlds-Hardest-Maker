public static class FieldModeExtension
{
    public static FieldMode GetFieldMode(this string tag)
    {
        foreach (FieldMode fieldMode in EditModeManager.Instance.AllFieldModes)
        {
            if (tag.Equals(fieldMode.Tag)) return fieldMode;
        }

        return null;
    }

    public static bool IsSolidFieldTag(this string tag) => tag.GetFieldMode().IsSolid;
}