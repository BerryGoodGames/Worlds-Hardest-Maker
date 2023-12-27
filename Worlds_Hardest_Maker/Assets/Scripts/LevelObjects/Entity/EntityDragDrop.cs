using System;
using UnityEngine;

/// <summary>
///     Makes entity drag and drop when shift is pressed
/// </summary>
public class EntityDragDrop : MonoBehaviour
{
    [SerializeField] private WorldPositionType worldType;
    public event Action<Vector2, Vector2> OnMove;

    protected virtual void OnMouseDrag()
    {
        if (EditModeManagerOther.Instance.Playing || !KeyBinds.GetKeyBind("Editor_MoveEntity")) return;

        Vector2 newPos = FollowMouse.GetCurrentMouseWorldPos(worldType);

        if (newPos == (Vector2)transform.position) return;

        Transform t = transform;
        Vector2 oldPos = t.position;

        t.position = newPos;

        OnMove?.Invoke(oldPos, newPos);
    }
}