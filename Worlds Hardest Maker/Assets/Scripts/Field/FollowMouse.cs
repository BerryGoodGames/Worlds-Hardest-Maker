using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public bool smooth = false;
    public float speed = 20;
    private void FixedUpdate()
    {
        if(!transform.position.Equals(GameManager.Instance.MousePosWorldSpaceRounded))
        {
            if (smooth)
                transform.position = Vector2.Lerp(transform.position, GameManager.Instance.MousePosWorldSpaceRounded, Time.fixedDeltaTime * speed);
            else
                transform.position = GameManager.Instance.MousePosWorldSpaceRounded;
        }
    }
}
