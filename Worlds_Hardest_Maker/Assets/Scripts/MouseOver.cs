using MyBox;
using UnityEngine;

/// <summary>
///     Checks if game objects colliders is hovered by mouse:
///     use mouseOver.Over
/// </summary>
public class MouseOver : MonoBehaviour
{
    [field: SerializeField]
    [field: ReadOnly]
    public bool Over { get; set; }

    [Space] [SerializeField] private bool updateChildrenEveryFrame;

    private Collider2D[] colliders;

    private void Start() => colliders = GetComponentsInChildren<Collider2D>();

    private void Update()
    {
        Vector2 mousePos = MouseManager.Instance.MouseWorldPos;
        if (updateChildrenEveryFrame) colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            if (!collider.bounds.Contains(mousePos)) continue;

            Over = true;
            return;
        }

        Over = false;
    }
}