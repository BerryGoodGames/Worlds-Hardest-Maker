using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour, IMouseService
{
    public Vector2? MouseDragStart { get; set; }
    public Vector2? MouseDragCurrent { get; set; }
    public Vector2? MouseDragEnd { get; set; }
    public Vector2 PrevMousePos { get; set; }
    public Vector2 MousePosDelta { get; set; } = Vector2.zero;
    private Vector2 mouseWorldPos = Vector2.positiveInfinity;
    
    public Vector2 MouseWorldPos
    {
        get
        {
            if (mouseWorldPos.Equals(Vector2.positiveInfinity)) mouseWorldPos = GetMouseWorldPos();
            
            return mouseWorldPos;
        }
        set => mouseWorldPos = value;
    }
    
    public Vector2 MouseCanvasPos => GameManager.ScreenToMainCanvas(Input.mousePosition);
    
    public Vector2 PrevMouseWorldPos { get; set; }
    public Vector2 MouseWorldPosGrid { get; set; }
    public Vector2 MouseWorldPosMatrix { get; set; }
    public bool IsOnScreen { get; set; } = true;
    public bool IsUIHovered { get; set; }
    public bool PrevMouseUp { get; set; }
    
    private Camera cam;
    private Vector2 GetMouseWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;

        return cam.ScreenToWorldPoint(mousePos);
        // if (cam != null) return cam.ScreenToWorldPoint(mousePos);
        // throw new Exception("Couldn't get mouse world position because main camera is null");
    }
    
    /// <summary>
    ///     Returns a tuple: (start of drag, end of drag);
    ///     <para>Exception when trying to access drag positions while they are null (-> no current dragging)</para>
    /// </summary>
    /// <exception cref="Exception"></exception>
    public (Vector2, Vector2) GetDragPositions()
    {
        if (MouseDragStart == null || MouseDragCurrent == null)
            throw new("Trying to access drag start and end positions when neither recorded");
        
        Vector2 start = (Vector2)MouseDragStart;
        Vector2 end = (Vector2)MouseDragCurrent;
        
        return (start.ConvertToGrid(), end.ConvertToGrid());
    }

    public Vector2 GetCurrentMouseWorldPos(WorldPositionType mode)
    {
        return mode switch
        {
            WorldPositionType.Any => MouseWorldPos,
            WorldPositionType.Grid => MouseWorldPosGrid,
            _ => MouseWorldPosMatrix,
        };
    }

    private void Update()
    {
        // check if UI is hovered
        IsUIHovered = EventSystem.current.IsPointerOverGameObject();
        
        // update position variables
        MouseWorldPosGrid = new(Mathf.Round(MouseWorldPos.x * 2) * 0.5f, Mathf.Round(MouseWorldPos.y * 2) * 0.5f);
        MouseWorldPosMatrix = new(Mathf.Round(MouseWorldPos.x), Mathf.Round(MouseWorldPos.y));
        
        // update drag variables
        if (KeyBinds.GetKeyBindDown("Editor_Select")) MouseDragStart = MouseWorldPos;
        if (KeyBinds.GetKeyBind("Editor_Select")) MouseDragCurrent = MouseWorldPos;
        if (KeyBinds.GetKeyBindUp("Editor_Select")) MouseDragEnd = MouseWorldPos;
        
        Vector2 view = cam.ScreenToViewportPoint(Input.mousePosition);
        IsOnScreen = view.x is > 0 and < 1 && view.y is > 0 and < 1;
        
        MousePosDelta = (Vector2)Input.mousePosition - PrevMousePos;
    }
    
    private void LateUpdate()
    {
        // set previous mouse pos
        PrevMousePos = Input.mousePosition;
        PrevMouseWorldPos = MouseWorldPos;
        MouseWorldPos = Vector2.positiveInfinity;
        PrevMouseUp = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    }

    private void Start()
    {
        if (cam == null) cam = Camera.main;
    }
}