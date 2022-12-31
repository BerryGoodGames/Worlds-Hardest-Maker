using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }

    [PunRPC]
    public void SetBall(float mx, float my, float bounceMx, float bounceMy, float speed)
    {
        Vector2 pos = new(mx, my);

        if (IsBallThere(mx, my)) return;

        // bounce pos is initialized locally based on ball object pos
        Vector2 bouncePos = new(mx + bounceMx, my + bounceMy);

        // instantiate prefab
        InstantiateBall(pos, bouncePos, speed, MultiplayerManager.Instance.Multiplayer);
    }

    [PunRPC]
    public void SetBall(float mx, float my)
    {
        Instance.SetBall(mx, my, 0, 0, 5);
    }

    public bool IsBallThere(float mx, float my)
    {
        return GetBalls(mx, my).Count > 0;
    }

    /// <summary>
    ///     Instantiates new ball default at (0, 0), also sends new instantiate request to photon network
    /// </summary>
    /// <param name="multiplayer"></param>
    /// <returns></returns>
    public GameObject InstantiateBall(Vector2 pos, Vector2 bouncePos, float speed, bool multiplayer)
    {
        GameObject newBall;
        if (multiplayer)
        {
            newBall = PhotonNetwork.Instantiate("BallDefault", Vector2.zero, Quaternion.identity);

            PhotonView view = newBall.transform.GetChild(0).GetComponent<PhotonView>();
            view.RPC("SetObjectPos", RpcTarget.All, pos);
            view.RPC("SetBouncePos", RpcTarget.All, bouncePos);
            view.RPC("SetSpeed", RpcTarget.All, speed);
        }
        else
        {
            newBall = Instantiate(PrefabManager.Instance.ballDefault, Vector2.zero, Quaternion.identity,
                ReferenceManager.Instance.ballDefaultContainer);

            BallDefaultController defaultController =
                newBall.transform.GetChild(0).GetComponent<BallDefaultController>();
            defaultController.SetObjectPos(pos);
            defaultController.SetBouncePos(bouncePos);
            defaultController.SetSpeed(speed);
        }

        return newBall;
    }

    [PunRPC]
    public void RemoveBall(float mx, float my)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.01f, 128);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out BallDefaultController b)) continue;

            if (b.startPosition.x.EqualsFloat(mx) && b.startPosition.y.EqualsFloat(my)) b.DestroyBall();
        }
    }

    public List<GameObject> GetBalls(float mx, float my)
    {
        List<GameObject> list = new();

        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.01f, 128);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out BallDefaultController b)) continue;

            if (b.startPosition.x.EqualsFloat(mx) && b.startPosition.y.EqualsFloat(my)) list.Add(b.gameObject);
        }

        return list;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}