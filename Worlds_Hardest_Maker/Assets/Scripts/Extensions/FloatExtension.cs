using System;
using UnityEngine;

public static class FloatExtension
{
    public static float RoundToNearestStep(this float value, float step) => Mathf.Round(value / step) * step;

    public static double RoundToNearestStep(this double value, double step) => Math.Round(value / step) * step;


    public static double Map(this double value, double start1, double stop1, double start2, double stop2)
    {
        double range1 = stop1 - start1;
        double range2 = stop2 - start2;

        return range2 / range1 * (value - start1) + start2;
    }

    public static float Map(this float value, float start1, float stop1, float start2, float stop2)
    {
        float range1 = stop1 - start1;
        float range2 = stop2 - start2;

        return range2 / range1 * (value - start1) + start2;
    }

    public static bool EqualsFloat(this float x, float y, float tolerance = 1e-10f)
    {
        float diff = Math.Abs(x - y);

        return diff <= tolerance || diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }
}