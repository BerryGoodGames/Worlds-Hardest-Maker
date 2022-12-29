using System;
using UnityEngine;

public static class UnitPixelUtils
{
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
}