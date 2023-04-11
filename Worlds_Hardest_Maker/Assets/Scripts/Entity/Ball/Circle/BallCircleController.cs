using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class BallCircleController : BallController
{
    [SerializeField] private float hoverDeviation;
    [SerializeField] private int startAnglePositionCount;

    [FormerlySerializedAs("origin")] [Space]
    public Transform Origin;

    [FormerlySerializedAs("hoverHint")] public Transform HoverHint;

    [FormerlySerializedAs("line")] [HideInInspector]
    public Transform Line;

    [FormerlySerializedAs("radius")] [Space] [HideInInspector]
    public float Radius;

    [FormerlySerializedAs("startAngle")] [HideInInspector]
    public float StartAngle;

    [FormerlySerializedAs("currentAngle")] [HideInInspector]
    public float CurrentAngle;

    private bool isDragging;

    private new void Awake()
    {
        base.Awake();

        if (transform.parent.parent != ReferenceManager.Instance.BallCircleContainer)
            transform.parent.SetParent(ReferenceManager.Instance.BallCircleContainer);

        InitCircleObject();
    }

    private void Update()
    {
        Vector2 mousePos = MouseManager.Instance.MouseWorldPos;
        bool mouseOver = BallCircleManager.PointOnCircle(mousePos, Origin.position, Radius, hoverDeviation);

        Origin.gameObject.SetActive(!EditModeManager.Instance.Playing);
        Line.gameObject.SetActive(!EditModeManager.Instance.Playing);

        UpdateHoverHintVisibility(mousePos, mouseOver);

        CheckUserInput(mousePos, mouseOver);

        // let ball circle around origin
        if (!EditModeManager.Instance.Playing) return;

        CurrentAngle -= GetAngularSpeed();
        UpdateAnglePos();
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private float GetAngularSpeed() => Speed * Time.deltaTime / Radius;

    private void UpdateHoverHintVisibility(Vector2 mousePos, bool mouseOver)
    {
        // set visibility of hover hint
        if (!EditModeManager.Instance.Playing && mouseOver &&
            (Input.GetKey(KeybindManager.Instance.BallCircleAngleKey) ||
             Input.GetKey(KeybindManager.Instance.BallCircleRadiusKey)))
        {
            // update pos of hover hint
            HoverHint.gameObject.SetActive(true);
            HoverHint.position = GetPointOnCircleFromRayPos(MouseManager.Instance.MouseWorldPos);
        }
        else
        {
            HoverHint.gameObject.SetActive(false);
        }
    }

    private void CheckUserInput(Vector2 mousePos, bool mouseOver)
    {
        // check for start angle or radius change from user
        if (!EditModeManager.Instance.Playing && isDragging &&
            (Input.GetKey(KeybindManager.Instance.BallCircleAngleKey) ||
             Input.GetKey(KeybindManager.Instance.BallCircleRadiusKey)))
        {
            // check start angle
            if (Input.GetMouseButton(0) && Input.GetKey(KeybindManager.Instance.BallCircleAngleKey) &&
                !EventSystem.current.IsPointerOverGameObject())
                SetStartAngle(GetAngleToOrigin(mousePos));

            if (Input.GetMouseButton(0) && Input.GetKey(KeybindManager.Instance.BallCircleRadiusKey) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                float mouseDist = Vector2.Distance(Origin.position, mousePos).RoundToNearestStep(0.5f);
                if (mouseDist > 0) SetRadius(mouseDist);
            }
        }

        // detect hover and drag
        if (!isDragging && Input.GetMouseButtonDown(0) && mouseOver) isDragging = true;
        if (Input.GetMouseButtonUp(0)) isDragging = false;
    }

    [PunRPC]
    public void MoveOrigin(float mx, float my)
    {
        Origin.position = new(mx, my);
        UpdateAnglePos();

        // update circle
        GameObject stroke = Line.gameObject;

        LineRenderer circle = stroke.GetComponent<LineRenderer>();

        const int steps = 100;
        List<Vector2> points = LineManager.GetCirclePoints(new(mx, my), Radius, steps);

        circle.positionCount = steps + 1;
        for (int i = 0; i < points.Count; i++) circle.SetPosition(i, points[i]);
    }

    [PunRPC]
    public void UpdateAnglePos()
    {
        Vector2 originPosition = Origin.position;
        Vector2 pos = new(
            originPosition.x + Mathf.Cos(CurrentAngle) * Radius,
            originPosition.y + Mathf.Sin(CurrentAngle) * Radius
        );
        transform.position = pos;
    }

    [PunRPC]
    public void SetRadius(float radius)
    {
        Radius = radius;

        InitCircleObject();
        UpdateAnglePos();
    }

    [PunRPC]
    public void SetStartAngle(float startAngle)
    {
        // round input to nearest position
        float angle = startAngle.RoundToNearestStep(Mathf.PI * 2 / startAnglePositionCount);

        StartAngle = angle;
        CurrentAngle = angle;
        transform.position = GetPointOnCircleFromAngle(angle);
    }

    [PunRPC]
    public void SetCurrentAngle(float angle)
    {
        CurrentAngle = angle;
    }

    /// <summary>Creates ray from origin to pos and returns the point intersecting with path circle</summary>
    public Vector2 GetPointOnCircleFromRayPos(Vector2 pos)
    {
        Vector2 diff = pos - (Vector2)Origin.position;

        float angle = -Vector2.SignedAngle(diff, Vector2.right) * Mathf.PI / 180;

        return GetPointOnCircleFromAngle(angle);
    }

    /// <returns>Point on circle path by angle in radians</returns>
    public Vector2 GetPointOnCircleFromAngle(float angle)
    {
        Vector2 originPosition = Origin.position;
        float resX = originPosition.x + Mathf.Cos(angle) * Radius;
        float resY = originPosition.y + Mathf.Sin(angle) * Radius;
        return new(resX, resY);
    }

    /// <returns>Angle from pos to origin in radians</returns>
    public float GetAngleToOrigin(Vector2 pos) =>
        -Vector2.SignedAngle(pos - (Vector2)Origin.position, Vector2.right) * Mathf.PI / 180;

    public void InitCircleObject()
    {
        if (Line != null) Destroy(Line.gameObject);

        // set circle
        LineManager.SetFill(0, 0, 0);
        LineManager.SetWeight(0.11f);
        LineManager.SetOrderInLayer(-2);
        LineManager.SetLayerID(LineManager.BallLayerID);
        GameObject circleObj = LineManager.DrawCircle(Origin.position, Radius, transform.parent);

        Line = circleObj.transform;
    }

    [PunRPC]
    public override void MoveObject(Vector2 unitPos, int id)
    {
        MoveOrigin(unitPos.x, unitPos.y);
    }

    public override Vector2 GetPosition() => Origin.position;

    public override Data GetData() => new BallCircleData(this);
}