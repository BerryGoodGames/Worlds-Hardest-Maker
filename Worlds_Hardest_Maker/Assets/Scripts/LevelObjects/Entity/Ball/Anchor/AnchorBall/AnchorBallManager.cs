using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour
{
    public static AnchorBallManager Instance { get; private set; }

    [ReadOnly] public List<AnchorBallController> AnchorBallList;
    [ReadOnly] public Dictionary<AnchorController, List<AnchorBallController>> AnchorBallListSheets;
    [ReadOnly] public List<AnchorBallController> AnchorBallListGlobal;

    #region Set, Get

    public static AnchorBallController SetAnchorBall(Vector2 position) => SetAnchorBallInSheet(position, PlaceManager.GetCurrentSheet());

    public static AnchorBallController SetAnchorBallInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        if (GetAnchorBallInSheet(position, sheet) != null) return null;

        Transform container = AnchorAttachManager.Instance.InAttachMode 
            ? AnchorAttachManager.GetCurrentAnchorContainer() 
            : ReferenceManager.Instance.AnchorBallContainer.transform;
        
        // instantiate
        GameObject ball = Instantiate(
            PrefabManager.Instance.AnchorBall, 
            container.position, Quaternion.identity, 
            container
        );
        AnchorBallController ballController = ball.GetComponentInChildren<AnchorBallController>();

        // setup parent
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            ballController.ParentAnchor = sheet;
            sheet.Balls.Add(ball.transform);
        }

        ballController.transform.position = position;

        // track ball positions in all the layers
        Instance.AnchorBallList.Add(ballController);

        if (AnchorAttachManager.Instance.InAttachMode) Instance.AnchorBallListSheets[AnchorManager.Instance.SelectedAnchor].Add(ballController);
        else Instance.AnchorBallListGlobal.Add(ballController);

        PlaceManager.AttachToSheet(ball, sheet);

        return ballController;
    }

    public static List<AnchorBallController> GetAnchorBalls(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.01f, LayerManager.Instance.Layers.Entity);
        List<AnchorBallController> res = new();

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("AnchorBallObject")) continue;

            res.Add(hit.GetComponent<AnchorBallController>());
        }

        return res;
    }

    public static AnchorBallController GetAnchorBallInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("AnchorBallObject")) continue;
            if (!hit.TryGetComponent(out AnchorBallController ball)) continue;
            if (IsAnchorBallInSheet(ball, sheet)) return ball;
        }

        return null;
    }
    
    public static bool IsAnchorBallInSheet(AnchorBallController anchorBall, [CanBeNull] AnchorController sheet)
    {
        bool hasAttachment = anchorBall.transform.parent.TryGetComponent(out AnchorAttachment attachment);
        
        // shorthand to:
        bool globalSheet = sheet == null;
        if (hasAttachment && globalSheet) return false;
        if (hasAttachment && attachment.Anchor != sheet) return false;
        if (!hasAttachment && !globalSheet) return false;
        return true;

        // return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }

    #endregion

    public static void SelectAnchorBall(Vector2 position)
    {
        // check if anchor ball there
        List<AnchorBallController> ballsAtPos = GetAnchorBalls(position);

        if (ballsAtPos.Count == 0) return;

        // get first ball at position and (de)select corresponding anchor
        foreach (AnchorBallController ball in ballsAtPos)
        {
            if (!ball.IsParentAnchorNull && ball.ParentAnchor.transform.position == ball.transform.position) continue;

            if (ball.IsParentAnchorNull || AnchorManager.Instance.SelectedAnchor == ball.ParentAnchor) AnchorManager.Instance.DeselectAnchor();
            else AnchorManager.Instance.SelectAnchor(ball.ParentAnchor, false);

            break;
        }
    }

    private void Start()
    {
        AnchorBallListSheets = new();
        AnchorBallListGlobal = new();

        PlayManager.Instance.OnSwitchToPlay += ReferenceManager.Instance.AnchorBallContainer.BallFadeIn;
        PlayManager.Instance.OnSwitchToEdit += () =>
        {
            if (AnchorManager.Instance.SelectedAnchor != null) ReferenceManager.Instance.AnchorBallContainer.BallFadeOut();
        };
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}