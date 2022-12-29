using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool smooth;
    [SerializeField] private float speed = 20;

    public WorldPosition worldPosition;
    // ANY explains itself, GRID only round or half positions, MATRIX only round positions

    public enum WorldPosition
    {
        ANY,
        GRID,
        MATRIX
    }

    private void FixedUpdate()
    {
        Vector2 pos = GetCurrentMouseWorldPos(worldPosition);
        if (transform.position.Equals(pos)) return;

        transform.position = smooth ? Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * speed) : pos;
    }

    public static Vector2 GetCurrentMouseWorldPos(WorldPosition mode)
    {
        return mode switch
        {
            WorldPosition.ANY => MouseManager.Instance.MouseWorldPos,
            WorldPosition.GRID => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix
        };
    }
}