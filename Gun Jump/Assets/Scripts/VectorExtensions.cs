using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Limited(this Vector2 vector, float maxLength)
    {
        if (vector.magnitude > maxLength)
        {
            vector = vector.normalized * maxLength;
        }

        return vector;
    }
}
