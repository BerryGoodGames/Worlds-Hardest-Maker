using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour
{
    public static AnchorBallManager Instance { get; set; }

    #region Set

    public static void SetAnchorBall(Vector2 pos, [CanBeNull] AnchorController anchor)
    {
        Transform container = anchor == null ? ReferenceManager.Instance.AnchorBallContainer : anchor.BallContainer;

        GameObject ball = Instantiate(PrefabManager.Instance.AnchorBall, Vector3.zero, Quaternion.identity, container);

        if (anchor != null)
        {
            ball.GetComponentInChildren<AnchorBallController>().ParentAnchor = anchor;
            anchor.Balls.Add(ball.transform);
        }
        
        ball.transform.GetChild(0).position = pos;
    }

    public static void SetAnchorBall(Vector2 pos)
    {
        AnchorController selectedAnchor = AnchorManager.Instance.SelectedAnchor;

        SetAnchorBall(pos, selectedAnchor);
    }

    public static void SetAnchorBall(float mx, float my) => SetAnchorBall(new(mx, my));

    public static void SetAnchorBall(float mx, float my, AnchorController anchor) => SetAnchorBall(new(mx, my), anchor);

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

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}