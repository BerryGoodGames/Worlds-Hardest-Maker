using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class BallManager : MonoBehaviour, IManager<BallController>, IManagerSelectable
{
    public static BallManager Instance { get; private set; }

    [FormerlySerializedAs("AnchorBallList")] [ReadOnly] public List<BallController> BallList;
    [ReadOnly] public Dictionary<AnchorController, List<BallController>> BallListSheets;
    [FormerlySerializedAs("AnchorBallListGlobal")] [ReadOnly] public List<BallController> BallListGlobal;

    #region Set, Get

    public Transform DefaultContainer => ReferenceManager.Instance.BallContainer;

    public BallController SetInSheet(ManagerParameters args)
    {
        if (GetInSheet(args.Position, args.Sheet) != null) return null;

        BallController ballController = InstantiateInSheet(args);

        // setup parent
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            ballController.ParentAnchor = args.Sheet;
            args.Sheet.Balls.Add(ballController.LevelObject.transform);
        }

        ballController.transform.position = args.Position;

        // track ball positions in all the layers
        Instance.BallList.Add(ballController);

        if (AnchorAttachManager.Instance.InAttachMode) Instance.BallListSheets[AnchorManager.Instance.SelectedAnchor].Add(ballController);
        else Instance.BallListGlobal.Add(ballController);

        PlaceManager.AttachToSheet(ballController.LevelObject, args.Sheet);

        return ballController;
    }

    public BallController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("BallObject")) continue;
            if (!hit.TryGetComponent(out BallController ball)) continue;
            if (IManager.IsInSheet(ball.transform.parent, sheet)) return ball;
        }

        return null;
    }

    public BallController InstantiateInSheet(ManagerParameters args)
    {
        Transform container = args.Sheet == null ? DefaultContainer : args.Sheet.AttachmentContainer;

        GameObject ball = Instantiate(
            PrefabManager.Instance.Ball,
            args.Position, Quaternion.identity,
            container
        );

        return ball.GetComponentInChildren<BallController>();
    }

    private static List<BallController> GetBalls(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.01f, LayerManager.Instance.Layers.Entity);
        List<BallController> res = new();

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("BallObject")) continue;

            res.Add(hit.GetComponent<BallController>());
        }

        return res;
    }

    #endregion

    public void Select(Vector2 position)
    {
        // check if ball there
        List<BallController> ballsAtPos = GetBalls(position);

        if (ballsAtPos.Count == 0) return;

        // get first ball at position and (de)select corresponding anchor
        foreach (BallController ball in ballsAtPos)
        {
            if (!ball.IsParentAnchorNull && ball.ParentAnchor.transform.position == ball.transform.position) continue;

            if (ball.IsParentAnchorNull || AnchorManager.Instance.SelectedAnchor == ball.ParentAnchor) AnchorManager.Instance.DeselectAnchor();
            else AnchorManager.Instance.Select(ball.ParentAnchor, false);

            break;
        }
    }

    private void Start()
    {
        BallListSheets = new();
        BallListGlobal = new();
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Ball;
}