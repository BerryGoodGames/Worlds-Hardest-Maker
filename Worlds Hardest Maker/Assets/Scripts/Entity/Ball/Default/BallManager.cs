using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static void SetBall(int mx, int my, int bounceMx = 0, int bounceMy = 0, float speed = 5f)
    {
        Vector2 pos = new(mx, my);
        // bounce pos is initialized locally based on ball object pos
        Vector2 bouncePos = new(mx + bounceMx, my + bounceMy);

        GameObject ball = GameManager.Instance.BallDefault;

        // instantiate prefab
        GameObject levelObject = Instantiate(ball, Vector2.zero, Quaternion.identity, GameManager.Instance.BallDefaultContainer.transform);

        // apply changes to prefab: pos, bounce pos, speed, target, line
        GameObject ballObject = levelObject.transform.GetChild(0).gameObject;
        GameObject bounce = levelObject.transform.GetChild(1).gameObject;

        // set position
        ballObject.transform.position = pos;
        bounce.transform.position = bouncePos;

        // set speed and target
        BallController controller = ballObject.GetComponent<BallController>();
        controller.startPosition = ballObject.transform.position;
        controller.currentTarget = bounce.transform.position;
        controller.speed = speed;

        // set line
        LineManager.SetFill(0, 0, 0);
        LineManager.SetWeight(0.11f);
        LineManager.DrawLine(ballObject.transform.position, bounce.transform.position, 201, levelObject.transform);
    }

    public static void RemoveBall(int mx, int my)
    {
        GameObject container = GameManager.Instance.BallDefaultContainer;
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.4f);
        foreach(Collider2D hit in hits)
        {
            if(hit.gameObject.name == "BallObject" && hit.gameObject.transform.parent.parent == container.transform)
            {
                Destroy(hit.gameObject.transform.parent.GetChild(0).GetComponent<AppendSlider>().GetSliderObject());
                Destroy(hit.gameObject.transform.parent.gameObject);
                break;
            }
        }
    }
}
