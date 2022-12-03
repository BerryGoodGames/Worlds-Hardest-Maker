using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

    [HideInInspector] public Vector2? MouseDragStart { get; set; } = null;
    [HideInInspector] public Vector2? MouseDragCurrent { get; set; } = null;
    [HideInInspector] public Vector2? MouseDragEnd { get; set; } = null;
    [HideInInspector] public Vector2 PrevMousePos { get; set; }
    [HideInInspector] public Vector2 MousePosDelta { get; set; } = Vector2.zero;
    private Vector2 mouseWorldPos = Vector2.positiveInfinity;
    [HideInInspector] public Vector2 MouseWorldPos { get
        {
            if(mouseWorldPos.Equals(Vector2.positiveInfinity))
            {
                mouseWorldPos = GetMouseWorldPos();
            }
            return (Vector2)mouseWorldPos;
        } private set 
        { 
            mouseWorldPos = value;
        } }
    [HideInInspector] public Vector2 PrevMouseWorldPos { get; set; } = new();
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
        if (Instance.MouseDragStart == null || Instance.MouseDragCurrent == null) throw new System.Exception("Trying to access drag start and end positions when neither recorded");
        
        Vector2 start = (Vector2)Instance.MouseDragStart;
        Vector2 end = (Vector2)Instance.MouseDragCurrent;

        return (start.ConvertPosition(worldPosition), end.ConvertPosition(worldPosition));
    }

    private void Update()
    {
        // update position variables
        MouseWorldPosGrid = new(Mathf.Round(MouseWorldPos.x * 2) * 0.5f, Mathf.Round(MouseWorldPos.y * 2) * 0.5f);
        MouseWorldPosMatrix = new(Mathf.Round(MouseWorldPos.x), Mathf.Round(MouseWorldPos.y));

        // update drag variables
        if (Input.GetMouseButtonDown(KeybindManager.Instance.SelectionMouseButton)) Instance.MouseDragStart = Instance.MouseWorldPos;
        if (Input.GetMouseButton(KeybindManager.Instance.SelectionMouseButton)) Instance.MouseDragCurrent = Instance.MouseWorldPos;
        if (Input.GetMouseButtonUp(KeybindManager.Instance.SelectionMouseButton)) Instance.MouseDragEnd = Instance.MouseWorldPos;

        Vector2 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        OnScreen = view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1;

        MousePosDelta = (Vector2)Input.mousePosition - PrevMousePos;
    }

    private void LateUpdate()
    {
        // set previous mouse pos
        Instance.PrevMousePos = Input.mousePosition;
        Instance.PrevMouseWorldPos = Instance.MouseWorldPos;
        Instance.MouseWorldPos = Vector2.positiveInfinity;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
