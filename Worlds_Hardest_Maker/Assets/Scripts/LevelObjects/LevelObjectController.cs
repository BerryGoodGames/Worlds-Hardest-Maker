using MyBox;
using UnityEngine;

public abstract class LevelObjectController : MonoBehaviour
{
    [ReadOnly] public bool IsAttached;
    
    public abstract EditMode EditMode { get; }
    
    public abstract Data GetData();
    
    public virtual void OnAnchorMove(Vector2 oldPos, Vector2 newPos) { }
    
    public virtual void Delete()
    {
        AudioManager.Instance.Play(PlaceManager.Instance.GetSfx(EditModeManager.Delete));
        Destroy(gameObject);
    }
    
    public static bool TryGetController(Component component, out LevelObjectController controller) =>
        component.TryGetComponent(out controller)
        || (controller = component.GetComponentInChildren<LevelObjectController>()) != null
        || (controller = component.GetComponentInParent<LevelObjectController>()) != null;
}