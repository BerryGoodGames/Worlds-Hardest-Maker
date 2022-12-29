using System;
using UnityEngine;

/// <summary>
///     makes entity drag and drop when shift is pressed
///     TODO: this is shit
/// </summary>
public class EntityDragDrop : MonoBehaviour
{
    [SerializeField] private bool halfGrid;
    public event Action onMove;

    private void OnMouseDrag()
    {
        if (GameManager.Instance.Playing || !Input.GetKey(KeybindManager.Instance.EntityMoveKey)) return;

        Vector2 newPos = halfGrid ? MouseManager.Instance.MouseWorldPosGrid : MouseManager.Instance.MouseWorldPosMatrix;
        if (newPos != (Vector2)transform.position)
        {
            transform.position = newPos;

            onMove?.Invoke();
        }

        if (TryGetComponent(out PathController controller))
        {
            controller.UpdateStartingPosition();
        }
    }
}