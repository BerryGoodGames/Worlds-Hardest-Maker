using Photon.Pun;
using UnityEngine;

public class BallDefaultController : BallController
{
    public Transform Bounce;

    [HideInInspector] public Transform Line;
    [HideInInspector] public Vector2 StartPosition;
    [HideInInspector] public Vector2 CurrentTarget;

    private void Start()
    {
        if (transform.parent.parent != ReferenceManager.Instance.BallDefaultContainer)
            transform.parent.SetParent(ReferenceManager.Instance.BallDefaultContainer);

        InitLine();
    }

    private void OnDestroy() => Destroy(transform.parent.gameObject);

    private void Update()
    {
        Bounce.gameObject.SetActive(!EditModeManager.Instance.Playing);
        Line.gameObject.SetActive(!EditModeManager.Instance.Playing);

        if (EditModeManager.Instance.Playing)
            Move();
        else
            UpdateLine();
    }

    private void InitLine()
    {
        Transform t = transform;

        // set line
        DrawManager.SetFill(0, 0, 0);
        DrawManager.SetWeight(0.11f);
        DrawManager.SetOrderInLayer(2);
        DrawManager.SetLayerID(DrawManager.BallLayerID);
        Line = DrawManager.DrawLine(t.position, Bounce.position, t.parent).transform;
    }

    private void Move()
    {
        // switch target after bounce
        if ((Vector2)transform.position == CurrentTarget)
        {
            if (CurrentTarget == (Vector2)Bounce.position)
                CurrentTarget = StartPosition;
            else if (CurrentTarget == (Vector2)transform.position) CurrentTarget = Bounce.position;
        }

        // move towards the target
        transform.position = Vector2.MoveTowards(transform.position, CurrentTarget, Speed * Time.deltaTime);
    }

    [PunRPC]
    public void SetObjectPos(Vector2 pos)
    {
        CurrentTarget = Bounce.position;
        StartPosition = pos;
        transform.position = pos;
    }

    [PunRPC]
    public void SetBouncePos(Vector2 pos)
    {
        CurrentTarget = pos;
        Bounce.position = pos;
    }

    public void SetLinePoint(int index, Vector2 point)
    {
        GameObject stroke = Line.gameObject;
        LineRenderer renderer = stroke.GetComponent<LineRenderer>();

        renderer.SetPosition(index, point);
    }

    public void UpdateLine()
    {
        SetLinePoint(0, transform.position);
        SetLinePoint(1, Bounce.position);
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
            SetObjectPos(unitPos);
        else
            SetBouncePos(unitPos);
    }

    public override Data GetData() => new BallData(this);
}