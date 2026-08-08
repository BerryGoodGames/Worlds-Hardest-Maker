using UnityEngine;

public interface IMouseService
{
    public Vector2? DragStart { get; set; }
    public Vector2? DragCurrent { get; set; }
    public Vector2? MouseDragEnd { get; set; }
    public Vector2 PrevMousePos { get; set; }
    public Vector2 MousePosDelta { get; set; }
    public Vector2 MouseWorldPos { get; set; }
    public Vector2 MouseCanvasPos { get; }
    public Vector2 PrevMouseWorldPos { get; set; }
    public Vector2 MouseWorldPosGrid { get; set; }
    public Vector2 MouseWorldPosMatrix { get; set; }
    public bool IsOnScreen { get; set; }
    public bool IsUIHovered { get; set; }
    public bool PrevMouseUp { get; set; }

    public (Vector2, Vector2) GetDragPositions();
    public Vector2 GetCurrentMouseWorldPos(WorldPositionType mode);
}