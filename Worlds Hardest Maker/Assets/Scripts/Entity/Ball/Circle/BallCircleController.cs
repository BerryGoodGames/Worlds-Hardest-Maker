using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCircleController : IBallController
{
    public override float SpeedMin { get { return -10; } }
    public override float SpeedMax { get { return 10; } }

    [HideInInspector] public int radius;
    [HideInInspector] public float startAngle;
    [HideInInspector] public float currentAngle;

    private void Update()
    {
        GetOrigin().SetActive(!GameManager.Instance.Playing);
        GetLine().SetActive(!GameManager.Instance.Playing);

        // let ball circle around origin
        if (GameManager.Instance.Playing)
        {
            currentAngle -= GetAngularSpeed();
            UpdateAnglePos();
        }
    }

    private float GetAngularSpeed()
    {
        return speed * Time.deltaTime / radius;
    }

    public void MoveOrigin(int mx, int my)
    {
        GetOrigin().transform.position = new(mx, my);
        UpdateAnglePos();

        // update circle
        GameObject stroke = GetLine();

        LineRenderer circle = stroke.GetComponent<LineRenderer>();

        int steps = 100;
        List<Vector2> points = LineManager.GetCirclePoints(new(mx, my), radius, steps);

        circle.positionCount = steps + 1;
        for (int i = 0; i < points.Count; i++)
        {
            circle.SetPosition(i, points[i]);
        }
    }

    public void UpdateAnglePos()
    {
        GameObject origin = GetOrigin();
        Vector2 pos = new(
            origin.transform.position.x + Mathf.Cos(currentAngle) * radius,
            origin.transform.position.y + Mathf.Sin(currentAngle) * radius);
        transform.position = pos;
    }

    public GameObject GetOrigin()
    {
        return transform.parent.GetChild(1).gameObject;
    }
    public Vector2 GetOriginPos()
    {
        return GetOrigin().transform.position;
    }
    public GameObject GetLine()
    {
        return transform.parent.GetChild(2).gameObject;
    }

    public override void MoveObject(Vector2 unitPos, GameObject movedObject)
    {
        base.MoveObject(unitPos, movedObject);
        MoveOrigin((int)unitPos.x, (int)unitPos.y);
    }
}
