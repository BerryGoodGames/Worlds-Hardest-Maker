using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBall : IBallController
{
    public Transform bounce;
    [HideInInspector] public Transform line;
    [HideInInspector] public Vector2 startPosition;
    [HideInInspector] public Vector2 currentTarget;

    private void Start()
    {
        if(transform.parent.parent != MGame.Instance.BallDefaultContainer.transform)
        {
            transform.parent.SetParent(MGame.Instance.BallDefaultContainer.transform);
        }

        // set line
        MLine.SetFill(0, 0, 0);
        MLine.SetWeight(0.11f);
        MLine.SetOrderInLayer(2);
        MLine.SetLayerID(MLine.BallLayerID);
        GameObject line = MLine.DrawLine(transform.position, bounce.position, transform.parent);

        this.line = line.transform;
    }

    private void Update()
    {
        bounce.gameObject.SetActive(!MGame.Instance.Playing);
        line.gameObject.SetActive(!MGame.Instance.Playing);

        if (MGame.Instance.Playing)
        {
            Move();
        } 
        else
        {
            UpdateLine();
        }
    }

    private void Move()
    {
        // switch target after bounce
        if((Vector2)transform.position == currentTarget)
        {
            if(currentTarget == (Vector2)bounce.position)
            {
                currentTarget = startPosition;
            } 
            else if(currentTarget == (Vector2)transform.position)
            {
                currentTarget = bounce.position;
            }
        }

        // move towards the target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }

    [PunRPC]
    public void SetObjectPos(Vector2 pos)
    {
        currentTarget = bounce.position;
        startPosition = pos;
        transform.position = pos;
    }

    [PunRPC]
    public void SetBouncePos(Vector2 pos) {
        currentTarget = pos;
        bounce.position = pos;
    }

    public void SetLinePoint(int index, Vector2 point)
    {
        GameObject stroke = line.gameObject;
        LineRenderer renderer = stroke.GetComponent<LineRenderer>();

        renderer.SetPosition(index, point);
    }
    public void UpdateLine()
    {
        SetLinePoint(0, transform.position);
        SetLinePoint(1, bounce.position);
    }

    public void DestroyBall()
    {
        Destroy(GetComponent<AppendSlider>().GetSliderObject());
        Destroy(transform.parent.gameObject);
    }

    [PunRPC]
    public override void MoveObject(Vector2 unitPos, int id)
    {
        GameObject movedObject = BallDragDrop.DragDropList[id];

        // call correct method to set position, either set object or set bounce
        if (movedObject.Equals(gameObject))
        {
            SetObjectPos(unitPos);
        } else
        {
            SetBouncePos(unitPos);
        }
    }
}
