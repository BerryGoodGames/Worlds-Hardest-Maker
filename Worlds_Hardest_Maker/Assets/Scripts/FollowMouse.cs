using UnityEngine;
using UnityEngine.Serialization;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool smooth;
    [SerializeField] private float speed = 20;

    [FormerlySerializedAs("WorldPosition")] public WorldPositionType WorldPosition;
    // ANY explains itself, GRID only round or half positions, MATRIX only round positions

    public enum WorldPositionType
    {
        ANY,
        GRID,
        MATRIX
    }

    private void FixedUpdate()
    {
        Vector2 pos = GetCurrentMouseWorldPos(WorldPosition);
        if (transform.position.Equals(pos)) return;

        transform.position = smooth ? Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * speed) : pos;
    }

    public static Vector2 GetCurrentMouseWorldPos(WorldPositionType mode)
    {
        return mode switch
        {
            WorldPositionType.ANY => MouseManager.Instance.MouseWorldPos,
            WorldPositionType.GRID => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix
        };
    }
}