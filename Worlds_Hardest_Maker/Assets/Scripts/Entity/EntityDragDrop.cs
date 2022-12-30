using System;
using UnityEngine;

/// <summary>
///     makes entity drag and drop when shift is pressed
///     this is shit
/// </summary>
public class EntityDragDrop : MonoBehaviour
{
    [SerializeField] private bool halfGrid;
    public event Action OnMove;

    private void OnMouseDrag()
    {
        if (EditModeManager.Instance.Playing || !Input.GetKey(KeybindManager.Instance.entityMoveKey)) return;

        Vector2 newPos = halfGrid ? MouseManager.Instance.MouseWorldPosGrid : MouseManager.Instance.MouseWorldPosMatrix;
        if (newPos != (Vector2)transform.position)
        {
            transform.position = newPos;

            OnMove?.Invoke();
        }

        if (TryGetComponent(out PathController controller))
        {
            controller.UpdateStartingPosition();
        }
    }
}