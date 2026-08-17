using System;
using UnityEngine;
using VContainer;

/// <summary>
///     Makes entity drag and drop when shift is pressed
/// </summary>
public class EntityDragDrop : MonoBehaviour
{
    [SerializeField] private WorldPositionType worldType;
    public event Action<Vector2, Vector2> OnMove;
    
    [Inject] private IMouseService mouseService;
    
    protected virtual void OnMouseDrag()
    {
        if (LevelSessionEditManager.Instance.IsPlaying || !KeyBinds.GetKeyBind("Editor_MoveEntity")) return;
        
        Vector2 newPos = mouseService.GetCurrentMouseWorldPos(worldType);
        
        if (newPos == (Vector2)transform.position) return;
        
        Transform t = transform;
        Vector2 oldPos = t.position;
        
        t.position = newPos;
        
        OnMove?.Invoke(oldPos, newPos);
    }
}