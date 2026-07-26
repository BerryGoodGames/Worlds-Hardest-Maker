using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool smooth;
    [SerializeField] private float speed = 20;
    
    public WorldPositionType WorldPosition;
    // ANY explains itself, GRID only round or half positions, MATRIX only round positions
    
    public bool AllowSnapToNewCell { get; set; } = true;
    
    private Vector2 targetPos;
    
    private void Update()
    {
        Vector2 pos = GetCurrentMouseWorldPos(WorldPosition);
        
        if (AllowSnapToNewCell && !pos.Equals(targetPos))
        {
            targetPos = pos;
        }
        
        if (!transform.position.Equals(targetPos))
        {
            transform.position = smooth ? Vector2.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * speed) : targetPos;
        }
    }
    
    public static Vector2 GetCurrentMouseWorldPos(WorldPositionType mode) =>
        mode switch
        {
            WorldPositionType.Any => MouseManager.Instance.MouseWorldPos,
            WorldPositionType.Grid => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix,
        };
}