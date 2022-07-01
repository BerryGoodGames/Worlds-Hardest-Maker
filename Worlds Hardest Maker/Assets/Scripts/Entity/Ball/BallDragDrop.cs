using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * script for enabling drag and drop on ball objects, also bounce pos or origin pos
 * 
 * attach to gameobject to be dragged
 */
public class BallDragDrop : MonoBehaviour
{
    private void OnMouseDrag()
    {
        if (Input.GetKey(GameManager.Instance.BallDragKey))
        {
            Vector2 pos = GameManager.GetMouseWorldPos();
            Vector2 unitPos = new(Mathf.Round(pos.x), Mathf.Round(pos.y));

            IBallController controller = GetComponent<BallController>();
            if(controller == null)
            {
                controller = transform.parent.GetChild(0).GetComponent<BallController>();
            }
            if(controller == null)
            {
                controller = transform.parent.GetChild(0).GetComponent<BallCircleController>();
            }

            controller.MoveObject(unitPos, gameObject);
        }
    }
}
