using MyBox;
using UnityEngine;

public class SolidFieldColliderConfig : MonoBehaviour
{
    [SerializeField] [MustBeAssigned] private PolygonCollider2D polygonCollider;
    [Space] [SerializeField] [PositiveValueOnly] private float size;
    [SerializeField] [PositiveValueOnly] private float cornerCut;
    
    [ButtonMethod]
    public void Apply() =>
        polygonCollider.points = new Vector2[]
        {
            new(-size / 2, size / 2 - cornerCut),
            new(-size / 2, -size / 2 + cornerCut),
            new(-size / 2 + cornerCut, -size / 2),
            new(size / 2 - cornerCut, -size / 2),
            new(size / 2, -size / 2 + cornerCut),
            new(size / 2, size / 2 - cornerCut),
            new(size / 2 - cornerCut, size / 2),
            new(-size / 2 + cornerCut, size / 2),
        };
}