// File: LevelObjects/Field/Outline/OutlineGeometry.cs
using UnityEngine;

/// <summary>
///     Pure math for turning "connected or not, per direction" into line segment endpoints.
///     Used by both real field outlines and preview outlines so they can never visually diverge.
/// </summary>
public static class OutlineGeometry
{
    public static readonly Vector2[] Directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, };

    public static (Vector2 start, Vector2 end) GetLinePoints(
        Vector2 position, Vector2 localScale, float weight, Vector2 dir,
        bool leftConnected, bool rightConnected
    )
    {
        float halfWidth = localScale.x / 2;
        float halfHeight = localScale.y / 2;
        float halfWeight = weight / 2;

        if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
        {
            float y = position.y + dir.y * 0.5f - dir.y * halfWeight;
            float x1 = position.x - halfWidth - (leftConnected ? weight : 0);
            float x2 = position.x + halfWidth + (rightConnected ? weight : 0);

            return (new(x1, y), new(x2, y));
        }

        float x = position.x + dir.x * 0.5f - dir.x * halfWeight;

        return (new(x, position.y + halfHeight), new(x, position.y - halfHeight));
    }
}