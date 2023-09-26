using System;
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
        // List<AnchorBallController> ballsAtPos = GetAnchorBalls(pos);
        //
        // foreach (AnchorBallController ballAtPos in ballsAtPos)
        // {
        //     // check if there is ball at position with same layer/parent
        //     if (ballAtPos.ParentAnchor == parentAnchor) return;
        // }

        if (GetAnchorBall(pos, parentAnchor) != null) return;
        
        Transform container =
            parentAnchor == null ? ReferenceManager.Instance.AnchorBallContainer.transform : parentAnchor.BallContainer;

        GameObject ball = Instantiate(PrefabManager.Instance.AnchorBall, container.position, Quaternion.identity, container);
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

    public static void SetAnchorBall(Vector2 pos)
    {
        AnchorController selectedAnchor = AnchorManager.Instance.SelectedAnchor;

        SetAnchorBall(pos, selectedAnchor);
    }

    public static void SetAnchorBall(float mx, float my) => SetAnchorBall(new(mx, my));

    public static void SetAnchorBall(float mx, float my, [CanBeNull] AnchorController anchor) => SetAnchorBall(new(mx, my), anchor);

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
            if(ball.Position == pos) return ball;
        }

        return null;
    }

    #endregion

    #region Remove

    [PunRPC]
    public void RemoveAnchorBall(Vector2 pos)
    {
        // ReSharper disable once Unity.PreferNonAllocApi
        // Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.05f);
        //
        // foreach (Collider2D hit in hits)
        // {
        //     if (!hit.CompareTag("AnchorBallObject")) continue;
        //
        //     Destroy(hit.transform.parent.gameObject);
        // }
        AnchorBallListGlobal.ForEach(CheckAnchorBall);

        foreach (KeyValuePair<AnchorController, List<AnchorBallController>> anchorBallListPair in AnchorBallListLayers)
        {
            anchorBallListPair.Value.ForEach(CheckAnchorBall);
        }

        return;

        void CheckAnchorBall(AnchorBallController ball)
        {
            if (ball.transform.position.x != pos.x || ball.transform.position.y != pos.y) return;

            Destroy(ball.gameObject);
        }
    }

    [PunRPC]
    public void RemoveAnchorBall(float mx, float my) => RemoveAnchorBall(new(mx, my));

    #endregion

    public static void SelectAnchorBall(Vector2 pos)
    {
        List<AnchorBallController> ballsAtPos = GetAnchorBalls(pos);

        foreach (AnchorBallController ball in ballsAtPos)
        {
            // select parent anchor if ball has parent and the parent is not currently selected (avoid deselecting anchor)
            if (ball.ParentAnchor == null ||
                AnchorManager.Instance.SelectedAnchor == ball.ParentAnchor) continue;

            AnchorManager.Instance.SelectAnchor(ball.ParentAnchor);
            break;
        }
    }

    private void Start()
    {
        AnchorBallListLayers = new();
        AnchorBallListGlobal = new();
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}