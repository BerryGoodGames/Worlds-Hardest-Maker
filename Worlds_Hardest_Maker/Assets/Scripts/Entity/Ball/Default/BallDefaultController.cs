using Photon.Pun;
using UnityEngine;

public class BallDefaultController : BallController
{
    public Transform bounce;
    [HideInInspector] public Transform line;
    [HideInInspector] public Vector2 startPosition;
    [HideInInspector] public Vector2 currentTarget;

    private void Start()
    {
        if (transform.parent.parent != ReferenceManager.Instance.ballDefaultContainer)
        {
            transform.parent.SetParent(ReferenceManager.Instance.ballDefaultContainer);
        }

        InitLine();
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void Update()
    {
        bounce.gameObject.SetActive(!GameManager.Instance.Playing);
        line.gameObject.SetActive(!GameManager.Instance.Playing);

        if (GameManager.Instance.Playing)
        {
            Move();
        }
        else
        {
            UpdateLine();
        }
    }

    private void InitLine()
    {
        // set line
        LineManager.SetFill(0, 0, 0);
        LineManager.SetWeight(0.11f);
        LineManager.SetOrderInLayer(2);
        LineManager.SetLayerID(LineManager.ballLayerID);
        GameObject line = LineManager.DrawLine(transform.position, bounce.position, transform.parent);

        this.line = line.transform;
    }

    private void Move()
    {
        // switch target after bounce
        if ((Vector2)transform.position == currentTarget)
        {
            if (currentTarget == (Vector2)bounce.position)
            {
                currentTarget = startPosition;
            }
            else if (currentTarget == (Vector2)transform.position)
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
    public void SetBouncePos(Vector2 pos)
    {
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
        GameObject movedObject = BallDragDrop.dragDropList[id];

        // call correct method to set position, either set object or set bounce
        if (movedObject.Equals(gameObject))
        {
            SetObjectPos(unitPos);
        }
        else
        {
            SetBouncePos(unitPos);
        }
    }

    public override Data GetData()
    {
        return new BallData(this);
    }
}