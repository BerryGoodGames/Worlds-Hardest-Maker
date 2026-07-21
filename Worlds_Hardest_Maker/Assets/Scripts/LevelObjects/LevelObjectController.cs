using MyBox;
using UnityEngine;
using Zenject;

public abstract class LevelObjectController : MonoBehaviour
{
    [ReadOnly] public bool IsAttached;
    
    public abstract EditMode EditMode { get; }
    
    protected IAudioService audioService;
    
    [Inject]
    public void Construct(IAudioService audioService)
    {
        this.audioService = audioService;
    }
    
    public abstract Data GetData();
    
    public virtual void OnAnchorMove(Vector2 oldPos, Vector2 newPos) { }
    
    public virtual void Delete()
    {
        audioService.Play(PlaceManager.Instance.GetSfx(EditModeManager.Delete));
        Destroy(gameObject);
    }
    
    public static bool TryGetController(Component component, out LevelObjectController controller) =>
        component.TryGetComponent(out controller)
        || (controller = component.GetComponentInChildren<LevelObjectController>()) != null
        || (controller = component.GetComponentInParent<LevelObjectController>()) != null;
}