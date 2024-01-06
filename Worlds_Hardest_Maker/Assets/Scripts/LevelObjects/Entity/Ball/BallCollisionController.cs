using UnityEngine;

/// <summary>
///     Detects player death event
///     <para>Attach to ball objects</para>
/// </summary>
public class BallCollisionController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) => OnTriggerStay2D(other);

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (EditModeManagerOther.Instance.Editing || !collider.CompareTag("Player")) return;

        PlayerController controller = collider.GetComponent<PlayerController>();

        if (controller.InDeathAnim) return;

        if (!controller.IsOnSafeField()) controller.DieNormal();
    }
}