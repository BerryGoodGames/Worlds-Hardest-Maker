using System;

public static class EnumUtils
{
    public static TEnumTo ConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e) => (TEnumTo)Enum.Parse(typeof(TEnumTo), e.ToString());

    public static object TryConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        Enum.TryParse(typeof(TEnumTo), e.ToString(), out object convEnum);

        return convEnum;
    }

    public static T ParseString<T>(string input) => (T)Enum.Parse(typeof(T), input);
}