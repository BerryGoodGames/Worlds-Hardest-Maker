using System.Globalization;
using System.Threading;
using UnityEngine;

public static class Utils
{
    public static void ForceDecimalSeparator(string separator)
    {
        string cultureName = Thread.CurrentThread.CurrentCulture.Name;
        CultureInfo ci = new(cultureName);
        if (ci.NumberFormat.NumberDecimalSeparator == separator) return;

        // Forcing use of decimal separator for numerical values
        ci.NumberFormat.NumberDecimalSeparator = separator;
        Thread.CurrentThread.CurrentCulture = ci;
    }

    public static Vector2 GetScreenDimensions(Camera cam, float? zoom)
    {
        zoom ??= cam.orthographicSize;

        float height = 2 * (float)zoom;
        float width = cam.aspect * height;
        return new(width, height);
    }
}