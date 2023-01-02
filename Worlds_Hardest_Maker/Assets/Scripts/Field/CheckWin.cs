using UnityEngine;

/// <summary>
///     attach to (start-and-)goal field
/// </summary>
public class CheckWin : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!EditModeManager.Instance.Playing ||
            !collider.gameObject.TryGetComponent(out PlayerController controller)) return;

        // check if every coin is collected
        if (controller.inDeathAnim || controller.won || !controller.CoinsCollected()) return;

        controller.Win();
    }
}