using System;
using System.Globalization;
using System.Threading;
using UnityEngine;

public static class Utils
{
    #region Enum methods

    public static TEnumTo ConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        return (TEnumTo)Enum.Parse(typeof(TEnumTo), e.ToString());
    }

    public static object TryConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        Enum.TryParse(typeof(TEnumTo), e.ToString(), out object convEnum);

        return convEnum;
    }

    #endregion

    #region Float methods

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

    public static bool DoFloatsEqual(float x, float y, float tolerance = 1e-10f)
    {
        float diff = Math.Abs(x - y);

        return diff <= tolerance || diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }

    #endregion

    #region Unit Pixel conversion methods

    public static float PixelToUnit(float pixel)
    {
        Camera cam = Camera.main;
        if (cam != null) return pixel * 2 * cam.orthographicSize / cam.pixelHeight;
        throw new Exception($"Couldn't convert {pixel} pixels to units because main camera is null");
    }

    public static float PixelToUnit(float pixel, float ortho)
    {
        Camera cam = Camera.main;
        if (cam != null) return pixel * 2 * ortho / cam.pixelHeight;
        throw new Exception($"Couldn't convert {pixel} pixels to units because main camera is null");
    }

    public static Vector2 PixelToUnit(Vector2 pixel)
    {
        return new(PixelToUnit(pixel.x), PixelToUnit(pixel.y));
    }

    public static Vector2 PixelToUnit(Vector2 pixel, float ortho)
    {
        return new(PixelToUnit(pixel.x, ortho), PixelToUnit(pixel.y, ortho));
    }

    public static float UnitToPixel(float unit)
    {
        Camera cam = Camera.main;
        if (cam != null) return unit * cam.pixelHeight / (cam.orthographicSize * 2);
        throw new Exception($"Couldn't convert {unit} units to pixels because main camera is null");
    }

    public static Vector2 UnitToPixel(Vector2 unit)
    {
        return new(UnitToPixel(unit.x), UnitToPixel(unit.y));
    }

    public static Rect RtToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new((Vector2)transform.position - size * 0.5f, size);
    }

    #endregion

    public static void ForceDecimalSeparator(string separator)
    {
        string cultureName = Thread.CurrentThread.CurrentCulture.Name;
        CultureInfo ci = new(cultureName);
        if (ci.NumberFormat.NumberDecimalSeparator == separator) return;

        // Forcing use of decimal separator for numerical values
        ci.NumberFormat.NumberDecimalSeparator = separator;
        Thread.CurrentThread.CurrentCulture = ci;
    }
}