using Photon.Pun;
using UnityEngine;

public class AnchorBallManagerOld : MonoBehaviour
{
    public static AnchorBallManagerOld Instance { get; private set; }

    /// <summary>
    ///     place ball at position
    /// </summary>
    /// <param name="pos">position of ball</param>
    public static void SetAnchorBall(Vector2 pos)
    {
        if (AnchorManagerOld.Instance.SelectedAnchor == null) return;

        if (MultiplayerManager.Instance.Multiplayer)
        {
            PhotonView view = PhotonView.Get(AnchorManagerOld.Instance.SelectedAnchor);
            view.RPC("RPCSetBall", RpcTarget.All, pos);
        }
        else
        {
            GameObject ball = Instantiate(PrefabManager.Instance.ball, Vector2.zero, Quaternion.identity,
                AnchorManagerOld.Instance.SelectedAnchor.GetComponent<AnchorControllerOld>().container.transform);
            ball.transform.GetChild(0).position = pos;
        }
    }

    public static void SetAnchorBall(Vector2 pos, Transform container)
    {
        GameObject ball = Instantiate(PrefabManager.Instance.ball, Vector2.zero, Quaternion.identity, container);
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
            if (hit.CompareTag("BallObject"))
            {
                Destroy(hit.transform.parent.gameObject);
            }
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