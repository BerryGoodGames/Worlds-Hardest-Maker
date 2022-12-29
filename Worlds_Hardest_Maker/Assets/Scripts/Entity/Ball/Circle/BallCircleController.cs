using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallCircleController : BallController
{
    [SerializeField] private float hoverDeviation;
    [SerializeField] private int startAnglePositionCount;
    [Space] public Transform origin;
    public Transform hoverHint;
    [HideInInspector] public Transform line;
    [Space] [HideInInspector] public float radius;
    [HideInInspector] public float startAngle;
    [HideInInspector] public float currentAngle;

    private bool isDragging;

    private new void Awake()
    {
        base.Awake();

        if (transform.parent.parent != ReferenceManager.Instance.ballCircleContainer)
        {
            transform.parent.SetParent(ReferenceManager.Instance.ballCircleContainer);
        }

        InitCircleObject();
    }

    private void Update()
    {
        Vector2 mousePos = MouseManager.Instance.MouseWorldPos;
        bool mouseOver = BallCircleManager.PointOnCircle(mousePos, origin.position, radius, hoverDeviation);

        origin.gameObject.SetActive(!EditModeManager.Instance.Playing);
        line.gameObject.SetActive(!EditModeManager.Instance.Playing);

        UpdateHoverHintVisibility(mousePos, mouseOver);

        CheckUserInput(mousePos, mouseOver);

        // let ball circle around origin
        if (!EditModeManager.Instance.Playing) return;

        currentAngle -= GetAngularSpeed();
        UpdateAnglePos();
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private float GetAngularSpeed()
    {
        return speed * Time.deltaTime / radius;
    }

    private void UpdateHoverHintVisibility(Vector2 mousePos, bool mouseOver)
    {
        // set visibility of hover hint
        if (!EditModeManager.Instance.Playing && mouseOver &&
            (Input.GetKey(KeybindManager.Instance.ballCircleAngleKey) ||
             Input.GetKey(KeybindManager.Instance.ballCircleRadiusKey)))
        {
            // update pos of hover hint
            hoverHint.gameObject.SetActive(true);
            hoverHint.position = GetPointOnCircleFromRayPos(MouseManager.Instance.MouseWorldPos);
        }
        else
        {
            hoverHint.gameObject.SetActive(false);
        }
    }

    private void CheckUserInput(Vector2 mousePos, bool mouseOver)
    {
        // check for start angle or radius change from user
        if (!EditModeManager.Instance.Playing && isDragging &&
            (Input.GetKey(KeybindManager.Instance.ballCircleAngleKey) ||
             Input.GetKey(KeybindManager.Instance.ballCircleRadiusKey)))
        {
            // check start angle
            if (Input.GetMouseButton(0) && Input.GetKey(KeybindManager.Instance.ballCircleAngleKey) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                SetStartAngle(GetAngleToOrigin(mousePos));
            }

            if (Input.GetMouseButton(0) && Input.GetKey(KeybindManager.Instance.ballCircleRadiusKey) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                float mouseDist = Vector2.Distance(origin.position, mousePos).RoundToNearestStep(0.5f);
                if (mouseDist > 0)
                {
                    SetRadius(mouseDist);
                }
            }
        }

        // detect hover and drag
        if (!isDragging && Input.GetMouseButtonDown(0) && mouseOver) isDragging = true;
        if (Input.GetMouseButtonUp(0)) isDragging = false;
    }

    [PunRPC]
    public void MoveOrigin(float mx, float my)
    {
        origin.position = new(mx, my);
        UpdateAnglePos();

        // update circle
        GameObject stroke = line.gameObject;

        LineRenderer circle = stroke.GetComponent<LineRenderer>();

        const int steps = 100;
        List<Vector2> points = LineManager.GetCirclePoints(new(mx, my), radius, steps);

        circle.positionCount = steps + 1;
        for (int i = 0; i < points.Count; i++)
        {
            circle.SetPosition(i, points[i]);
        }
    }

    [PunRPC]
    public void UpdateAnglePos()
    {
        Vector2 originPosition = origin.position;
        Vector2 pos = new(
            originPosition.x + Mathf.Cos(currentAngle) * radius,
            originPosition.y + Mathf.Sin(currentAngle) * radius
        );
        transform.position = pos;
    }

    [PunRPC]
    public void SetRadius(float radius)
    {
        this.radius = radius;

        InitCircleObject();
        UpdateAnglePos();
    }

    [PunRPC]
    public void SetStartAngle(float startAngle)
    {
        // round input to nearest position
        float angle = startAngle.RoundToNearestStep(Mathf.PI * 2 / startAnglePositionCount);

        this.startAngle = angle;
        currentAngle = angle;
        transform.position = GetPointOnCircleFromAngle(angle);
    }

    [PunRPC]
    public void SetCurrentAngle(float angle)
    {
        currentAngle = angle;
    }

    /// <summary>"Creates" ray from origin to pos and returns the point intersecting with path circle</summary>
    /// <param name="pos"></param>
    public Vector2 GetPointOnCircleFromRayPos(Vector2 pos)
    {
        Vector2 diff = pos - (Vector2)origin.position;

        float angle = -Vector2.SignedAngle(diff, Vector2.right) * Mathf.PI / 180;

        return GetPointOnCircleFromAngle(angle);
    }

    /// <returns>Point on circle path by angle in radians</returns>
    public Vector2 GetPointOnCircleFromAngle(float angle)
    {
        Vector2 originPosition = origin.position;
        float resX = originPosition.x + Mathf.Cos(angle) * radius;
        float resY = originPosition.y + Mathf.Sin(angle) * radius;
        return new(resX, resY);
    }

    /// <returns>Angle from pos to origin in radians</returns>
    public float GetAngleToOrigin(Vector2 pos)
    {
        return -Vector2.SignedAngle(pos - (Vector2)origin.position, Vector2.right) * Mathf.PI / 180;
    }

    public void InitCircleObject()
    {
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        // set circle
        LineManager.SetFill(0, 0, 0);
        LineManager.SetWeight(0.11f);
        LineManager.SetOrderInLayer(-2);
        LineManager.SetLayerID(LineManager.ballLayerID);
        GameObject circleObj = LineManager.DrawCircle(origin.position, radius, transform.parent);

        line = circleObj.transform;
    }

    [PunRPC]
    public override void MoveObject(Vector2 unitPos, int id)
    {
        MoveOrigin(unitPos.x, unitPos.y);
    }

    public override Data GetData()
    {
        return new BallCircleData(this);
    }
}