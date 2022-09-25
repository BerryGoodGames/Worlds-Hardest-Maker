using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidDetection : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void FixedUpdate()
    {
        if (CheckCollision(transform.position))
        {
            // check corners
            int collisions = 0;

            for(int x = -1; x < 2 && collisions < 2; x += 2)
            {
                for(int y = -1; y < 2 && collisions < 2; y +=2)
                {
                    if(CheckCollision(new Vector2(transform.lossyScale.x * x * 0.5f, transform.lossyScale.y * y * 0.5f) + (Vector2)transform.position))
                    {
                        collisions++;
                    }
                }
            }

            if(collisions >= 2)
            {
                playerController.DieVoid();
            }
        }
    }

    /// <param name="position">position where to check</param>
    /// <returns>if there is a void at the position</returns>
    private bool CheckCollision(Vector2 position)
    {
        return Physics2D.OverlapPoint(position, 1024) != null;
    }
}
