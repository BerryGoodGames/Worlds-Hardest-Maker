using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

    [HideInInspector] public Vector2? MouseDragStart { get; set; } = null;
    [HideInInspector] public Vector2? MouseDragEnd { get; set; } = null;
    [HideInInspector] public Vector2 PrevMousePos { get; set; }
    [HideInInspector] public Vector2 MouseWorldPos { get; set; } = new();
    [HideInInspector] public Vector2 MouseWorldPosGrid { get; set; } = new();
    [HideInInspector] public Vector2 MouseWorldPosMatrix { get; set; } = new();
    [HideInInspector] public bool OnScreen { get; set; } = true;

    public static Vector2 PosToGrid(Vector2 pos) { return new(Mathf.Round(pos.x * 2) * 0.5f, Mathf.Round(pos.y * 2) * 0.5f); }
    public static Vector2 PosToMatrix(Vector2 pos) { return new(Mathf.Round(pos.x), Mathf.Round(pos.y)); }
    public static Vector2 GetMouseWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;

        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    /// <summary>
    /// Returns a tuple: (start of drag, end of drag);
    /// exception when trying to access drag positions while they are null (-> no current dragging)
    /// </summary>
    /// <param name="worldPosition">The worldposition mode, you want the output to be in (-> any, grid, matrix)</param>
    /// <exception cref="System.Exception"></exception>
    public static (Vector2, Vector2) GetDragPositions(FollowMouse.WorldPosition worldPosition)
    {
        if (Instance.MouseDragStart == null || Instance.MouseDragEnd == null) throw new System.Exception("Trying to access drag start and end positions when neither recorded");
        
        Vector2 start = (Vector2)Instance.MouseDragStart;
        Vector2 end = (Vector2)Instance.MouseDragEnd;

        return worldPosition == FollowMouse.WorldPosition.ANY ? (start, end) : 
            worldPosition == FollowMouse.WorldPosition.GRID ? (PosToGrid(start), PosToGrid(end)) : (PosToMatrix(start), PosToMatrix(end));
    }

    private void Update()
    {
        // update position variables
        MouseWorldPos = GetMouseWorldPos();
        MouseWorldPosGrid = new(Mathf.Round(MouseWorldPos.x * 2) * 0.5f, Mathf.Round(MouseWorldPos.y * 2) * 0.5f);
        MouseWorldPosMatrix = new(Mathf.Round(MouseWorldPos.x), Mathf.Round(MouseWorldPos.y));

        // update drag variables
        if (Input.GetMouseButtonDown(0)) Instance.MouseDragStart = Instance.MouseWorldPos;
        if (Input.GetMouseButton(0)) Instance.MouseDragEnd = Instance.MouseWorldPos;

        Vector2 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        OnScreen = view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1;
    }

    private void LateUpdate()
    {
        // set previous mouse pos
        Instance.PrevMousePos = Input.mousePosition;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
