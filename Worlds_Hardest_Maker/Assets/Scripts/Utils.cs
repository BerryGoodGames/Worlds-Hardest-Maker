using System;
using System.Globalization;
using System.Threading;
using UnityEngine;

public static class Utils
{
    public static TEnumTo ConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        return (TEnumTo)Enum.Parse(typeof(TEnumTo), e.ToString());
    }

    public static object TryConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        Enum.TryParse(typeof(TEnumTo), e.ToString(), out object convEnum);

        return convEnum;
    }

    public static float RoundToNearestStep(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }

    public static double Map(double value, double start1, double stop1, double start2, double stop2)
    {
        double range1 = stop1 - start1;
        double range2 = stop2 - start2;

        return range2 / range1 * (value - start1) + start2;
    }

    public static void ForceDecimalSeparator(string separator)
    {
        string cultureName = Thread.CurrentThread.CurrentCulture.Name;
        CultureInfo ci = new(cultureName);
        if (ci.NumberFormat.NumberDecimalSeparator == separator) return;

        // Forcing use of decimal separator for numerical values
        ci.NumberFormat.NumberDecimalSeparator = separator;
        Thread.CurrentThread.CurrentCulture = ci;
    }

    public static bool DoFloatsEqual(float x, float y, float tolerance = 1e-10f)
    {
        float diff = Math.Abs(x - y);

        return diff <= tolerance || diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }
}