using System.Globalization;
using System.Threading;

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
}