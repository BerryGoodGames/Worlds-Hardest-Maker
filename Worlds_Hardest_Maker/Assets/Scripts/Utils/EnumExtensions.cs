using System;

public static class EnumExtensions
{
    public static TEnumTo ConvertTo<TEnumFrom, TEnumTo>(this TEnumFrom e) where TEnumFrom : Enum where TEnumTo : Enum => (TEnumTo)Enum.Parse(typeof(TEnumTo), e.ToString());

    public static object TryConvertTo<TEnumFrom, TEnumTo>(this TEnumFrom e) where TEnumFrom : Enum where TEnumTo : Enum
    {
        Enum.TryParse(typeof(TEnumTo), e.ToString(), out object convEnum);

        return convEnum;
    }

    public static T ToEnum<T>(this string input) where T : Enum => (T)Enum.Parse(typeof(T), input);
}