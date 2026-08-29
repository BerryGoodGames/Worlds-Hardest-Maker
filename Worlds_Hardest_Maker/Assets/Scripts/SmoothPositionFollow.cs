using UnityEngine;

public class SmoothPositionFollow : MonoBehaviour
{
    public float Speed { get; set; } = 20;

    private Vector2 targetPosition;
    private bool hasTarget;

    public void SetTarget(Vector2 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    /// <summary>Sets the target and immediately jumps to it, skipping interpolation.</summary>
    public void SnapTo(Vector2 position)
    {
        targetPosition = position;
        hasTarget = true;
        transform.position = position;
    }

    private void Update()
    {
        if (!hasTarget) return;

        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * Speed);
    }
}