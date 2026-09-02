using UnityEngine;
using VContainer;

/// <summary>
///     Attach to goal field
/// </summary>
public class CheckWin : MonoBehaviour
{
    // TODO: check if injected
    [Inject] private ICoinManager coinManager;
    
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!LevelSessionEditManager.Instance.IsPlaying ||
            !collider.gameObject.TryGetComponent(out PlayerController controller)) return;
        
        // check if every coin is collected
        if (controller.InDeathAnim || controller.Won || !coinManager.AllCoinsCollected()) return;
        
        controller.Win();
    }
}