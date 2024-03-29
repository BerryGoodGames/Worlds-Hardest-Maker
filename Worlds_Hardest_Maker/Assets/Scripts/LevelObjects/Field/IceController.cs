using UnityEngine;

public class IceController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (!collider.CompareTag("Player")) return;

        PlayerController controller = collider.GetComponent<PlayerController>();

        if (!controller.IsOnIce())
            // frame player entered ice
            // transition between normal ground and ice
            // transfer velocity to player
        {
            controller.Rb.velocity = KeyBinds.GetMovementInput() *
                                     controller.Speed;
        }
    }
}