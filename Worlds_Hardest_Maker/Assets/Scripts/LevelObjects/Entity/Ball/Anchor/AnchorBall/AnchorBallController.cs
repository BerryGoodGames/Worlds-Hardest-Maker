using System.Collections.Generic;
using UnityEngine;

public class AnchorBallController : EntityController
{
    [HideInInspector] public AnchorController ParentAnchor;
    public bool IsParentAnchorNull { get; private set; }

    public override EditMode EditMode => EditModeManager.AnchorBall;

    public override Data GetData() => new AnchorBallData(StartPosition);

    [HideInInspector] public Vector2 StartPosition;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (ParentAnchor == null) IsParentAnchorNull = true;

        StartPosition = transform.localPosition;

        if (LevelSessionManager.Instance.IsEdit) PlayManager.Instance.OnSwitchToEdit += ResetPosition;
    }

    public void ResetPosition()
    {
        transform.localPosition = StartPosition;
        rb.velocity = Vector2.zero;
    }

    public override void Delete()
    {
        if (IsParentAnchorNull)
        {
            if (AnchorManager.Instance.SelectedAnchor == null) base.Delete();
        }
        else if (ParentAnchor.Selected) base.Delete();
    }

    private void OnDestroy()
    {
        AnchorBallManager.Instance.AnchorBallList.Remove(this);

        if (ParentAnchor != null)
        {
            ParentAnchor.Balls.Remove(transform.parent);

            // remove anchor ball from parent anchor cache list
            ref Dictionary<AnchorController, List<AnchorBallController>> ballList = ref AnchorBallManager.Instance.AnchorBallListLayers;
            if (ballList.ContainsKey(ParentAnchor)) ballList[ParentAnchor].Remove(this);
        }
        else AnchorBallManager.Instance.AnchorBallListGlobal.Remove(this);

        Destroy(transform.parent.gameObject);

        // unsubscribe
        if (LevelSessionManager.Instance.IsEdit) PlayManager.Instance.OnSwitchToEdit -= ResetPosition;
    }
}