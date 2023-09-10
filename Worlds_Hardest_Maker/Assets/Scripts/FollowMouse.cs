using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool smooth;
    [SerializeField] private float speed = 20;

    public WorldPositionType WorldPosition;
    // ANY explains itself, GRID only round or half positions, MATRIX only round positions

    public enum WorldPositionType
    {
        Any,
        Grid,
        Matrix
    }

    private void FixedUpdate()
    {
        Vector2 pos = GetCurrentMouseWorldPos(WorldPosition);
        if (transform.position.Equals(pos)) return;

        transform.position = smooth ? Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * speed) : pos;
    }

    public static Vector2 GetCurrentMouseWorldPos(WorldPositionType mode) =>
        mode switch
        {
            WorldPositionType.Any => MouseManager.Instance.MouseWorldPos,
            WorldPositionType.Grid => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix
        };
}