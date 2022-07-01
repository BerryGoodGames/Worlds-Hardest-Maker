using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : IBallController
{
    public override float SpeedMin { get { return 0; } }
    public override float SpeedMax { get { return 15; } }

    [HideInInspector] public Vector2 startPosition;
    [HideInInspector] public Vector2 currentTarget;

    private void Update()
    {
        GetBounce().SetActive(!GameManager.Instance.Playing);
        GetLine().SetActive(!GameManager.Instance.Playing);

        if (GameManager.Instance.Playing)
        {
            Move();
        } else
        {
            UpdateLine();
        }
    }

    void Move()
    {
        // move towards the target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        // switch target after bounce
        if((Vector2)transform.position == currentTarget)
        {
            if(currentTarget == GetBouncePos())
            {
                currentTarget = startPosition;
            } 
            else if(currentTarget == (Vector2)transform.position)
            {
                currentTarget = GetBouncePos();
            }
        }
    }

    public void ResetTarget()
    {
        currentTarget = GetBouncePos();
    }

    public GameObject GetBounce()
    {
        return transform.parent.GetChild(1).gameObject;
    }
    public Vector2 GetBouncePos()
    {
        return GetBounce().transform.position;
    }
    public void SetBouncePos(int mx, int my)
    {
        GetBounce().transform.position = new(mx, my);
    }
    public GameObject GetLine()
    {
        return transform.parent.GetChild(2).gameObject;
    }
    public void SetLinePoint(int index, Vector2 point)
    {
        GameObject stroke = GetLine();
        LineRenderer line = stroke.GetComponent<LineRenderer>();

        line.SetPosition(index, point);
    }
    public void UpdateLine()
    {
        SetLinePoint(0, transform.position);
        SetLinePoint(1, GetBouncePos());
    }

    public override void MoveObject(Vector2 unitPos, GameObject movedObject)
    {
        base.MoveObject(unitPos, movedObject);

        if (movedObject.Equals(gameObject))
        {
            startPosition = unitPos;
        }
    }
}
