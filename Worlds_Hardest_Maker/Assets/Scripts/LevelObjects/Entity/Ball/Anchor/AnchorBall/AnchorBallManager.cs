using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour, IManager<AnchorBallController>, IManagerSelectable
{
    public static AnchorBallManager Instance { get; private set; }

    [ReadOnly] public List<AnchorBallController> AnchorBallList;
    [ReadOnly] public Dictionary<AnchorController, List<AnchorBallController>> AnchorBallListSheets;
    [ReadOnly] public List<AnchorBallController> AnchorBallListGlobal;

    #region Set, Get

    public Transform DefaultContainer => ReferenceManager.Instance.AnchorBallContainer;

    public AnchorBallController SetInSheet(ManagerParameters args)
    {
        if (GetInSheet(args.Position, args.Sheet) != null) return null;

        AnchorBallController ballController = InstantiateInSheet(args);

        // setup parent
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            ballController.ParentAnchor = args.Sheet;
            args.Sheet.Balls.Add(ballController.LevelObject.transform);
        }

        ballController.transform.position = args.Position;

        // track ball positions in all the layers
        Instance.AnchorBallList.Add(ballController);

        if (AnchorAttachManager.Instance.InAttachMode) Instance.AnchorBallListSheets[AnchorManager.Instance.SelectedAnchor].Add(ballController);
        else Instance.AnchorBallListGlobal.Add(ballController);

        PlaceManager.AttachToSheet(ballController.LevelObject, args.Sheet);

        return ballController;
    }

    public AnchorBallController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("AnchorBallObject")) continue;
            if (!hit.TryGetComponent(out AnchorBallController ball)) continue;
            if (IManager.IsInSheet(ball.transform.parent, sheet)) return ball;
        }

        return null;
    }

    public AnchorBallController InstantiateInSheet(ManagerParameters args)
    {
        Transform container = args.Sheet == null ? DefaultContainer : args.Sheet.AttachmentContainer;

        GameObject ball = Instantiate(
            PrefabManager.Instance.AnchorBall,
            args.Position, Quaternion.identity,
            container
        );

        return ball.GetComponentInChildren<AnchorBallController>();
    }

    private static List<AnchorBallController> GetAnchorBalls(Vector2 pos)
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

    #endregion

    public void Select(Vector2 position)
    {
        // check if anchor ball there
        List<AnchorBallController> ballsAtPos = GetAnchorBalls(position);

        if (ballsAtPos.Count == 0) return;

        // get first ball at position and (de)select corresponding anchor
        foreach (AnchorBallController ball in ballsAtPos)
        {
            if (!ball.IsParentAnchorNull && ball.ParentAnchor.transform.position == ball.transform.position) continue;

            if (ball.IsParentAnchorNull || AnchorManager.Instance.SelectedAnchor == ball.ParentAnchor) AnchorManager.Instance.DeselectAnchor();
            else AnchorManager.Instance.Select(ball.ParentAnchor, false);

            break;
        }
    }

    private void Start()
    {
        AnchorBallListSheets = new();
        AnchorBallListGlobal = new();
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.AnchorBall;
}