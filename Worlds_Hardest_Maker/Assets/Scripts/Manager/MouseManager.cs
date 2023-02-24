using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

    #region Properties

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
            if (mouseWorldPos.Equals(Vector2.positiveInfinity))
            {
                mouseWorldPos = GetMouseWorldPos();
            }

            return mouseWorldPos;
        }
        private set => mouseWorldPos = value;
    }

    public Vector2 MouseCanvasPos => Input.mousePosition;

    public Vector2 PrevMouseWorldPos { get; set; }
    public Vector2 MouseWorldPosGrid { get; set; }
    public Vector2 MouseWorldPosMatrix { get; set; }
    public bool IsOnScreen { get; set; } = true;
    public bool IsUIHovered { get; set; }
    public bool PrevMouseUp { get; set; }

    #endregion

    #region Static methods

    public static Vector2 PosToGrid(Vector2 pos)
    {
        return new(Mathf.Round(pos.x * 2) * 0.5f, Mathf.Round(pos.y * 2) * 0.5f);
    }

    public static Vector2 PosToMatrix(Vector2 pos)
    {
        return new(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    private static Vector2 GetMouseWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;

        if (Camera.main != null) return Camera.main.ScreenToWorldPoint(mousePos);
        throw new Exception("Couldn't get mouse world position because main camera is null");
    }

    /// <summary>
    ///     Returns a tuple: (start of drag, end of drag);
    ///     <para>Exception when trying to access drag positions while they are null (-> no current dragging)</para>
    /// </summary>
    /// <param name="worldPositionType">The world position mode, you want the output to be in (-> any, grid, matrix)</param>
    /// <exception cref="Exception"></exception>
    public static (Vector2, Vector2) GetDragPositions(FollowMouse.WorldPositionType worldPositionType)
    {
        if (Instance.MouseDragStart == null || Instance.MouseDragCurrent == null)
            throw new Exception("Trying to access drag start and end positions when neither recorded");

        Vector2 start = (Vector2)Instance.MouseDragStart;
        Vector2 end = (Vector2)Instance.MouseDragCurrent;

        return (start.ConvertPosition(worldPositionType), end.ConvertPosition(worldPositionType));
    }

    #endregion

    private void Update()
    {
        // check if UI is hovered
        Instance.IsUIHovered = EventSystem.current.IsPointerOverGameObject();

        // update position variables
        MouseWorldPosGrid = new(Mathf.Round(MouseWorldPos.x * 2) * 0.5f, Mathf.Round(MouseWorldPos.y * 2) * 0.5f);
        MouseWorldPosMatrix = new(Mathf.Round(MouseWorldPos.x), Mathf.Round(MouseWorldPos.y));

        // update drag variables
        if (Input.GetMouseButtonDown(KeybindManager.Instance.SelectionMouseButton))
            Instance.MouseDragStart = Instance.MouseWorldPos;
        if (Input.GetMouseButton(KeybindManager.Instance.SelectionMouseButton))
            Instance.MouseDragCurrent = Instance.MouseWorldPos;
        if (Input.GetMouseButtonUp(KeybindManager.Instance.SelectionMouseButton))
            Instance.MouseDragEnd = Instance.MouseWorldPos;

        // ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector2 view = cam.ScreenToViewportPoint(Input.mousePosition);
            IsOnScreen = view.x is > 0 and < 1 && view.y is > 0 and < 1;
        }

        MousePosDelta = (Vector2)Input.mousePosition - PrevMousePos;
    }

    private void LateUpdate()
    {
        // set previous mouse pos
        Instance.PrevMousePos = Input.mousePosition;
        Instance.PrevMouseWorldPos = Instance.MouseWorldPos;
        Instance.MouseWorldPos = Vector2.positiveInfinity;
        Instance.PrevMouseUp = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}