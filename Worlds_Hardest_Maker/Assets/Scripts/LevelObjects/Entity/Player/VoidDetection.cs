using UnityEngine;

public class VoidDetection : MonoBehaviour
{
    private PlayerController playerController;

    private void Start() => playerController = GetComponent<PlayerController>();

    private void FixedUpdate()
    {
        if (playerController.InDeathAnim || !CheckVoidCollision(transform.position)) return;

        // check corners
        int collisionId = 0;
        int collisions = 0;

        for (int x = -1; x < 2 && (collisions < 2 || collisionId == 3); x += 2)
        {
            for (int y = -1; y < 2 && (collisions < 2 || collisionId == 3); y += 2)
            {
                if (!CheckVoidCollision(
                        new Vector2(transform.lossyScale.x * x * 0.5f, transform.lossyScale.y * y * 0.5f) +
                        (Vector2)transform.position)) continue;

                collisionId += (int)(Mathf.Clamp01(x) + Mathf.Clamp01(y) * 2);
                collisions++;
            }
        }

        if (collisions >= 2 && collisionId != 3) playerController.DieVoid();
    }

    /// <param name="position">position where to check</param>
    /// <returns>if there is a void at the position</returns>
    private static bool CheckVoidCollision(Vector2 position) =>
        Physics2D.OverlapPoint(position, LayerManager.Instance.Layers.Void) != null;
}