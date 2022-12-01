using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class BallCircleController : IBallController
{
    [SerializeField] private float hoverDeviation;
    [SerializeField] private int startAnglePositionCount;
    [Space]
    public Transform origin;
    public Transform hoverHint;
    [HideInInspector] public Transform line;
    [Space]
    [HideInInspector] public float radius;
    [HideInInspector] public float startAngle;
    [HideInInspector] public float currentAngle;
    private bool isDragging = false;

    private new void Awake()
    {
        base.Awake();

        if (transform.parent.parent != GameManager.Instance.BallCircleContainer.transform)
        {
            transform.parent.SetParent(GameManager.Instance.BallCircleContainer.transform);
        }

        InitCircleObject();
    }

    private void Update()
    {
        Vector2 mousePos = MouseManager.Instance.MouseWorldPos;
        bool mouseHovers = BallCircleManager.PointOnCircle(mousePos, origin.position, radius, hoverDeviation);
        
        origin.gameObject.SetActive(!GameManager.Instance.Playing);
        line.gameObject.SetActive(!GameManager.Instance.Playing);

        // TODO: improve
        // set visibility of hover hint
        if (!GameManager.Instance.Playing && mouseHovers &&
            (Input.GetKey(GameManager.Instance.BallCircleAngleKey) || Input.GetKey(GameManager.Instance.BallCircleRadiusKey)))
        {
            // update pos of hoverhint
            hoverHint.gameObject.SetActive(true);
            hoverHint.position = GetPointOnCircleFromRayPos(MouseManager.Instance.MouseWorldPos);
        }
        else
        {
            hoverHint.gameObject.SetActive(false);
        }

        // check for startangle or radius change from user
        if (!GameManager.Instance.Playing && isDragging &&
            (Input.GetKey(GameManager.Instance.BallCircleAngleKey) || Input.GetKey(GameManager.Instance.BallCircleRadiusKey)))
        {
            
            // check startangle
            if (Input.GetMouseButton(0) && Input.GetKey(GameManager.Instance.BallCircleAngleKey) && !EventSystem.current.IsPointerOverGameObject())
            {
                SetStartAngle(GetAngleToOrigin(mousePos));
            }

            if(Input.GetMouseButton(0) && Input.GetKey(GameManager.Instance.BallCircleRadiusKey) && !EventSystem.current.IsPointerOverGameObject())
            {
                float mouseDist = GameManager.RoundToNearestStep(Vector2.Distance(origin.position, mousePos), 0.5f);
                if(mouseDist > 0)
                {
                    SetRadius(mouseDist);
                }
            }
        }

        // detect hover and drag
        if (!isDragging && Input.GetMouseButtonDown(0) && mouseHovers) isDragging = true;
        if (Input.GetMouseButtonUp(0)) isDragging = false;

        // let ball circle around origin
        if (GameManager.Instance.Playing)
        {
            currentAngle -= GetAngularSpeed();
            UpdateAnglePos();
        }
    }
    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private float GetAngularSpeed()
    {
        return speed * Time.deltaTime / radius;
    }

    [PunRPC]
    public void MoveOrigin(float mx, float my)
    {
        origin.position = new(mx, my);
        UpdateAnglePos();

        // update circle
        GameObject stroke = line.gameObject;

        LineRenderer circle = stroke.GetComponent<LineRenderer>();

        int steps = 100;
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
        Vector2 pos = new(
            origin.position.x + Mathf.Cos(currentAngle) * radius,
            origin.position.y + Mathf.Sin(currentAngle) * radius
        );
        transform.position = pos;
    }

    [PunRPC]
    public void SetRadius(float radius) {
        this.radius = radius;

        InitCircleObject();
        UpdateAnglePos();
    }
    [PunRPC]
    public void SetStartAngle(float startAngle) {
        // round input to nearest position
        float angle = GameManager.RoundToNearestStep(startAngle, Mathf.PI * 2 / startAnglePositionCount);

        this.startAngle = angle;
        currentAngle = angle;
        transform.position = GetPointOnCircleFromAngle(angle);
    }
    [PunRPC]
    public void SetCurrentAngle(float angle) { currentAngle = angle; }

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
        float resX = origin.position.x + Mathf.Cos(angle) * radius;
        float resY = origin.position.y + Mathf.Sin(angle) * radius;
        return new(resX, resY);
    }
    /// <returns>Angle from pos to origin in radians</returns>
    public float GetAngleToOrigin(Vector2 pos) { return -Vector2.SignedAngle(pos - (Vector2)origin.position, Vector2.right) * Mathf.PI / 180; }

    public void InitCircleObject()
    {
        if(line != null)
        {
            Destroy(line.gameObject);
        }

        // set circle
        LineManager.SetFill(0, 0, 0);
        LineManager.SetWeight(0.11f);
        LineManager.SetOrderInLayer(-2);
        LineManager.SetLayerID(LineManager.BallLayerID);
        GameObject circleObj = LineManager.DrawCircle(origin.position, radius, transform.parent);

        line = circleObj.transform;
    }

    [PunRPC]
    public override void MoveObject(Vector2 unitPos, int id)
    {
        MoveOrigin(unitPos.x, unitPos.y);
    }

    public override IData GetData()
    {
        return new BallCircleData(this);
    }
}
