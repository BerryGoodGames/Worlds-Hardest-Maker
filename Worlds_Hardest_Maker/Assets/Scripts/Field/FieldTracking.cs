using UnityEngine;

/// <summary>
///     Apply this to every field which the player can stand on
/// </summary>
public class FieldTracking : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (!collider.CompareTag("Player")) return;

        PlayerController controller = collider.GetComponent<PlayerController>();
        controller.currentFields.Add(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (!collider.CompareTag("Player")) return;

        PlayerController controller = collider.GetComponent<PlayerController>();
        controller.currentFields.Remove(gameObject);
    }
}