using UnityEngine;

public class PickManager : MonoBehaviour
{
    public static PickManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public static void PickObject(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, LayerManager.Instance.Layers.LevelObjectMask);
        
        if (hits.Length == 0) return;
        
        if (!LevelObjectController.TryGetController(hits[0], out LevelObjectController levelObject))
        {
            Debug.Log("Object that was tried to pick from is not an entity");
            return;
        }
        
        LevelSessionEditManager.Instance.CurrentEditMode = levelObject.EditMode;
    }
}