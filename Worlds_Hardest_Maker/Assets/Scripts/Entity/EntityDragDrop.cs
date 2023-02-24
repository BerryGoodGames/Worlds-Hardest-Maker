using System;
using UnityEngine;

/// <summary>
///     Makes entity drag and drop when shift is pressed
/// </summary>
public class EntityDragDrop : MonoBehaviour
{
    [SerializeField] private FollowMouse.WorldPositionType worldType;
    public event Action OnMove;

    private void OnMouseDrag()
    {
        if (EditModeManager.Instance.Playing || !Input.GetKey(KeybindManager.Instance.EntityMoveKey)) return;

        Vector2 newPos = FollowMouse.GetCurrentMouseWorldPos(worldType);
        if (newPos != (Vector2)transform.position)
        {
            transform.position = newPos;

            OnMove?.Invoke();
        }

        if (TryGetComponent(out PathControllerOld controller))
        {
            controller.UpdateStartingPosition();
        }
    }
}