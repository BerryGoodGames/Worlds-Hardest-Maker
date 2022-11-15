using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MAnchorBall : MonoBehaviour
{
    public static MAnchorBall Instance { get; private set; }

    /// <summary>
    /// place ball at position
    /// </summary>
    /// <param name="pos">position of ball</param>
    public static void SetAnchorBall(Vector2 pos)
    {
        if (MAnchor.Instance.SelectedAnchor == null) return;

        if(MGame.Instance.Multiplayer)
        {
            PhotonView view = PhotonView.Get(MAnchor.Instance.SelectedAnchor);

            view.RPC("RPCSetBall", RpcTarget.All, pos);
        }
        else
        {
            GameObject ball = Instantiate(MGame.Instance.Ball, Vector2.zero, Quaternion.identity, MAnchor.Instance.SelectedAnchor.GetComponent<CAnchor>().container.transform);
            ball.transform.GetChild(0).position = pos;
        }
    }
    public static void SetAnchorBall(Vector2 pos, Transform container)
    {
        GameObject ball = Instantiate(MGame.Instance.Ball, Vector2.zero, Quaternion.identity, container);
        ball.transform.GetChild(0).position = pos;
    }
    public static void SetAnchorBall(float mx, float my) { SetAnchorBall(new(mx, my)); }
    public static void SetAnchorBall(float mx, float my, Transform container) { SetAnchorBall(new(mx, my), container); }

    [PunRPC]
    public void RemoveAnchorBall(Vector2 pos)
    {
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
