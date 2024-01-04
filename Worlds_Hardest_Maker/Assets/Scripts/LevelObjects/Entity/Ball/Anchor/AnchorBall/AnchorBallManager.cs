using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour
{
    public static AnchorBallManager Instance { get; private set; }

    [ReadOnly] public List<AnchorBallController> AnchorBallList;
    [ReadOnly] public Dictionary<AnchorController, List<AnchorBallController>> AnchorBallListLayers;
    [ReadOnly] public List<AnchorBallController> AnchorBallListGlobal;

    #region Set, Get

    public static AnchorBallController SetAnchorBall(Vector2 pos, [CanBeNull] AnchorController parentAnchor)
    {
        if (GetAnchorBall(pos, parentAnchor) != null) return null;

        bool hasParent = parentAnchor != null;

        // assign container
        Transform container = hasParent ? parentAnchor.BallContainer : ReferenceManager.Instance.AnchorBallContainer.transform;

        // instantiate
        GameObject ball = Instantiate(PrefabManager.Instance.AnchorBall, container.position, Quaternion.identity, container);
        AnchorBallController ballController = ball.GetComponentInChildren<AnchorBallController>();

        // setup parent
        if (hasParent)
        {
            ballController.ParentAnchor = parentAnchor;
            parentAnchor.Balls.Add(ball.transform);
        }

        ballController.transform.position = pos;

        // track ball positions in all the layers
        Instance.AnchorBallList.Add(ballController);

        if (hasParent) Instance.AnchorBallListLayers[parentAnchor].Add(ballController);
        else Instance.AnchorBallListGlobal.Add(ballController);

        return ballController;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static AnchorBallController SetAnchorBall(Vector2 position)
    {
        AnchorController selectedAnchor = AnchorManager.Instance.SelectedAnchor;
        return SetAnchorBall(position, selectedAnchor);
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

    public static AnchorBallController GetAnchorBall(Vector2 pos, AnchorController parentAnchor)
    {
        List<AnchorBallController> balls = parentAnchor == null
            ? Instance.AnchorBallListGlobal
            : Instance.AnchorBallListLayers[parentAnchor];

        foreach (AnchorBallController ball in balls)
        {
            if (ball.Position == pos) return ball;
        }

        return null;
    }

    #endregion

    public static void SelectAnchorBall(Vector2 position)
    {
        // check if anchor ball there
        List<AnchorBallController> ballsAtPos = GetAnchorBalls(position);

        if (ballsAtPos.Count <= 0) return;

        // get first ball at position and (de)select corresponding anchor
        foreach (AnchorBallController ball in ballsAtPos)
        {
            if (!ball.IsParentAnchorNull && ball.ParentAnchor.Position == ball.Position) continue;

            if (ball.IsParentAnchorNull || AnchorManager.Instance.SelectedAnchor == ball.ParentAnchor) AnchorManager.Instance.DeselectAnchor();
            else AnchorManager.Instance.SelectAnchor(ball.ParentAnchor, false);

            break;
        }
    }

    private void Start()
    {
        AnchorBallListLayers = new();
        AnchorBallListGlobal = new();

        EditModeManagerOther.Instance.OnPlay += ReferenceManager.Instance.AnchorBallContainer.BallFadeIn;
        EditModeManagerOther.Instance.OnEdit += () =>
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