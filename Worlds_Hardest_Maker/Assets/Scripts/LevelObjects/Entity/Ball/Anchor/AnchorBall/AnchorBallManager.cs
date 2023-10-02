using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour
{
    public static AnchorBallManager Instance { get; set; }

    public Dictionary<AnchorController, List<AnchorBallController>> AnchorBallListLayers;
    public List<AnchorBallController> AnchorBallListGlobal;

    #region Set

    public static void SetAnchorBall(Vector2 pos, [CanBeNull] AnchorController parentAnchor)
    {
        if (GetAnchorBall(pos, parentAnchor) != null) return;

        Transform container =
            parentAnchor == null ? ReferenceManager.Instance.AnchorBallContainer.transform : parentAnchor.BallContainer;

        GameObject ball = Instantiate(PrefabManager.Instance.AnchorBall, container.position, Quaternion.identity,
            container);
        AnchorBallController ballController = ball.GetComponentInChildren<AnchorBallController>();

        if (parentAnchor != null)
        {
            ballController.ParentAnchor = parentAnchor;
            parentAnchor.Balls.Add(ball.transform);
        }

        ball.transform.GetChild(0).position = pos;

        // track ball positions in all the layers
        if (parentAnchor != null) Instance.AnchorBallListLayers[parentAnchor].Add(ballController);
        else Instance.AnchorBallListGlobal.Add(ballController);
    }

    public static void SetAnchorBall(Vector2 position)
    {
        AnchorController selectedAnchor = AnchorManager.Instance.SelectedAnchor;

        SetAnchorBall(position, selectedAnchor);
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

    #region Remove

    [PunRPC]
    public void RemoveAnchorBall(Vector2 position)
    {
        AnchorBallListGlobal.ForEach(CheckAnchorBall);

        foreach (KeyValuePair<AnchorController, List<AnchorBallController>> anchorBallListPair in AnchorBallListLayers)
        {
            anchorBallListPair.Value.ForEach(CheckAnchorBall);
        }

        return;

        void CheckAnchorBall(AnchorBallController ball)
        {
            if ((Vector2)ball.transform.position != position) return;

            Destroy(ball.gameObject);
        }
    }

    #endregion

    public static void SelectAnchorBall(Vector2 position)
    {
        List<AnchorBallController> ballsAtPos = GetAnchorBalls(position);

        if (ballsAtPos.Count <= 0) return;

        foreach (AnchorBallController ball in ballsAtPos)
        {
            if (!ball.IsParentAnchorNull && ball.ParentAnchor.Position == ball.Position) continue;

            if (ball.IsParentAnchorNull || AnchorManager.Instance.SelectedAnchor == ball.ParentAnchor)
            {
                AnchorManager.Instance.DeselectAnchor();
            }
            else
            {
                AnchorManager.Instance.SelectAnchor(ball.ParentAnchor, false);
            }

            break;
        }
    }

    private void Start()
    {
        AnchorBallListLayers = new();
        AnchorBallListGlobal = new();

        EditModeManager.Instance.OnPlay += ReferenceManager.Instance.AnchorBallContainer.BallFadeIn;
        EditModeManager.Instance.OnEdit += () =>
        {
            if (AnchorManager.Instance.SelectedAnchor != null)
                ReferenceManager.Instance.AnchorBallContainer.BallFadeOut();
        };
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}