using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCircleManager : MonoBehaviour
{
    public static void SetBallCircle(int mx, int my, int r = 1, float speed = 0, float startAngle = Mathf.PI / 2)
    {
        Vector2 originPos = new(mx, my);

        GameObject ball = GameManager.Instance.BallCircle;

        // instantiate prefab
        GameObject levelObject = Instantiate(ball, Vector2.zero, Quaternion.identity, GameManager.Instance.BallCircleContainer.transform);

        GameObject ballObject = levelObject.transform.GetChild(0).gameObject;
        GameObject ballOrigin = levelObject.transform.GetChild(1).gameObject;

        // set position
        ballOrigin.transform.position = originPos;

        // set speed, radius, angle
        BallCircleController controller = ballObject.GetComponent<BallCircleController>();
        controller.speed = speed;
        controller.radius = r;
        controller.startAngle = startAngle;
        controller.currentAngle = startAngle;
        controller.UpdateAnglePos();

        // set circle
        LineManager.SetFill(0, 0, 0);
        LineManager.SetWeight(0.11f);
        LineManager.DrawCircle(mx, my, r, 99, levelObject.transform);
    }

    public static void RemoveBallCircle(int mx, int my)
    {
        GameObject container = GameManager.Instance.BallCircleContainer;
        foreach (Transform bc in container.transform)
        {
            Vector2 originPos = bc.GetChild(1).position;

            if (originPos.x == mx && originPos.y == my)
            {
                Destroy(bc.GetChild(0).GetComponent<AppendSlider>().GetSliderObject());
                Destroy(bc.gameObject);
            }
        }
    }
}
