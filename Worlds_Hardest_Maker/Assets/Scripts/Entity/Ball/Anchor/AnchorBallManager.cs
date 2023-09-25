using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour
{
    public static AnchorBallManager Instance { get; set; }

    #region Set

    public static void SetAnchorBall(Vector2 pos, [CanBeNull] AnchorController parentAnchor)
    {
        List<AnchorBallController> ballsAtPos = GetAnchorBalls(pos);

        foreach (AnchorBallController ballAtPos in ballsAtPos)
        {
            // check if there is ball at position with same layer/parent
            if (ballAtPos.ParentAnchor == parentAnchor) return;
        }
        // print(string.Join(" ", ballsAtPos));
        
        Transform container =
            parentAnchor == null ? ReferenceManager.Instance.AnchorBallContainer.transform : parentAnchor.BallContainer;

        GameObject ball = Instantiate(PrefabManager.Instance.AnchorBall, container.position, Quaternion.identity, container);
        
        if (parentAnchor != null)
        {
            ball.GetComponentInChildren<AnchorBallController>().ParentAnchor = parentAnchor;
            parentAnchor.Balls.Add(ball.transform);
        }

        ball.transform.GetChild(0).position = pos;
        // print(string.Join(" ", GetAnchorBalls(pos)));
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

    #endregion

    #region Remove

    [PunRPC]
    public void RemoveAnchorBall(Vector2 pos)
    {
        // ReSharper disable once Unity.PreferNonAllocApi
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.05f);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("AnchorBallObject")) continue;

            Destroy(hit.transform.parent.gameObject);
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

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}