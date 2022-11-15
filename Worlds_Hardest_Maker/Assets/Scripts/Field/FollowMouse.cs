using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool smooth = false;
    [SerializeField] private float speed = 20;
    public WorldPosition worldPosition;
    // ANY explains itself, GRID only round or half positions, MATRIX only round positions
    public enum WorldPosition
    {
        ANY, GRID, MATRIX
    }
    private void FixedUpdate()
    {
        Vector2 pos = GetCurrentMouseWorldPos(worldPosition);
        if (!transform.position.Equals(pos))
        {
            if (smooth)
                transform.position = Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * speed);
            else
                transform.position = pos;
        }
    }

    public static Vector2 GetCurrentMouseWorldPos(WorldPosition mode)
    {
        return mode == WorldPosition.ANY ? MouseManager.Instance.MouseWorldPos :
                      mode == WorldPosition.GRID ? MouseManager.Instance.MouseWorldPosGrid :
                      MouseManager.Instance.MouseWorldPosMatrix;
    }
}
