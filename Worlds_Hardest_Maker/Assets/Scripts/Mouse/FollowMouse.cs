using UnityEngine;
using VContainer;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool smooth;
    [SerializeField] private float speed = 20;
    
    public WorldPositionType WorldPosition;
    public bool AllowSnapToNewCell { get; set; } = true;

    private Vector2 targetPosition;
    public Vector2 TargetPosition => targetPosition;
    
    [Inject] private IMouseService mouseService;

    private void Awake() => targetPosition = transform.position; // avoid a (0,0) target before the first Update

    private void Update()
    {
        Vector2 pos = mouseService.GetCurrentMouseWorldPos(WorldPosition);
        if (AllowSnapToNewCell && !pos.Equals(targetPosition)) targetPosition = pos;
        if (!transform.position.Equals(targetPosition))
            transform.position = smooth ? Vector2.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * speed) : targetPosition;
    }
}