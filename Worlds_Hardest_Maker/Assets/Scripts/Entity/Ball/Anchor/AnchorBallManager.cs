using Photon.Pun;
using UnityEngine;

public class AnchorBallManager : MonoBehaviour
{
    public static AnchorBallManager Instance { get; set; }

    /// <summary>
    ///     Places ball at position
    /// </summary>
    public static void SetAnchorBall(Vector2 pos)
    {
        if (AnchorManager.Instance.SelectedAnchor == null) return;

        Debug.LogWarning("Anchor ball place multiplayer TODO");

        GameObject ball = Instantiate(PrefabManager.Instance.Ball, Vector2.zero, Quaternion.identity,
            AnchorManager.Instance.SelectedAnchor.BallContainer);
        ball.transform.GetChild(0).position = pos;
    }

    public static void SetAnchorBall(Vector2 pos, Transform container)
    {
        GameObject ball = Instantiate(PrefabManager.Instance.Ball, Vector2.zero, Quaternion.identity, container);
        ball.transform.GetChild(0).position = pos;
    }

    public static void SetAnchorBall(float mx, float my)
    {
        SetAnchorBall(new(mx, my));
    }

    public static void SetAnchorBall(float mx, float my, Transform container)
    {
        SetAnchorBall(new(mx, my), container);
    }

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
    public void RemoveAnchorBall(float mx, float my)
    {
        RemoveAnchorBall(new(mx, my));
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}