using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallCircleManager : MonoBehaviour
{
    public static BallCircleManager Instance { get; private set; }

    [PunRPC]
    public void SetBallCircle(float mx, float my, float r, float speed, float startAngle)
    {
        Vector2 originPos = new(mx, my);

        if (!IsBallCircleThere(mx, my))
        {
            // instantiate prefab
            InstantiateBallCircle(originPos, r, speed, startAngle);
        }
    }

    [PunRPC]
    public void SetBallCircle(float mx, float my)
    {
        SetBallCircle(mx, my, 1, 0, Mathf.PI * 0.5f);
    }

    public bool IsBallCircleThere(float mx, float my)
    {
        return GetBallCircles(mx, my).Count > 0;
    }

    public List<GameObject> GetBallCircles(float mx, float my)
    {
        List<GameObject> list = new();

        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.01f, 32768);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("BallCircleOrigin")) list.Add(hit.transform.parent.GetChild(0).gameObject);
        }

        return list;
    }

    private static GameObject InstantiateBallCircle(Vector2 pos, float r, float speed, float startAngle)
    {
        GameObject newBallCircle;
        if (GameManager.Instance.Multiplayer)
        {
            newBallCircle = PhotonNetwork.Instantiate("BallCircle", Vector2.zero, Quaternion.identity);

            PhotonView view = newBallCircle.transform.GetChild(0).GetComponent<PhotonView>();
            view.RPC("SetRadius", RpcTarget.All, r);
            view.RPC("MoveOrigin", RpcTarget.All, pos.x, pos.y);
            view.RPC("SetSpeed", RpcTarget.All, speed);
            view.RPC("SetStartAngle", RpcTarget.All, startAngle);
            view.RPC("SetCurrentAngle", RpcTarget.All, startAngle);
            view.RPC("UpdateAnglePos", RpcTarget.All);
        }
        else
        {
            newBallCircle = Instantiate(PrefabManager.Instance.BallCircle, Vector2.zero, Quaternion.identity,
                ReferenceManager.Instance.BallCircleContainer);

            BallCircleController controller = newBallCircle.transform.GetChild(0).GetComponent<BallCircleController>();
            controller.SetRadius(r);
            controller.MoveOrigin(pos.x, pos.y);
            controller.SetSpeed(speed);
            controller.SetStartAngle(startAngle);
            controller.SetCurrentAngle(startAngle);
            controller.UpdateAnglePos();
        }

        return newBallCircle;
    }

    [PunRPC]
    public void RemoveBallCircle(float mx, float my)
    {
        Transform container = ReferenceManager.Instance.BallCircleContainer;
        foreach (Transform bc in container)
        {
            Vector2 originPos = bc.GetChild(0).GetComponent<BallCircleController>().origin.position;

            if (originPos.x != mx || originPos.y != my) continue;

            Destroy(bc.GetChild(0).GetComponent<AppendSlider>().GetSliderObject());
            Destroy(bc.gameObject);
        }
    }

    public static bool PointOnCircle(Vector2 pos, Vector2 origin, float r, float deviation)
    {
        float maxDist = r + deviation;
        float minDist = r - deviation;
        float dist = Vector2.Distance(pos, origin);
        return dist >= minDist && dist < maxDist;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}