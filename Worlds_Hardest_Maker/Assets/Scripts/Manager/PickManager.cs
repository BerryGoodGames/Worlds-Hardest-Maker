using UnityEngine;

public class PickManager : MonoBehaviour
{
    public static PickManager Instance { get; private set; }

    private static int levelObjectMask = LayerMask.GetMask("Entity", "Field", "Player", "Void", "Water");

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static void PickObject(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, levelObjectMask);

        if (hits.Length <= 0) return;

        if (!hits[0].TryGetComponent(out EntityController entity))
        {
            throw new("Object that was tried to pick from is not an entity");
        }
        
        EditModeManagerOther.Instance.CurrentEditMode = entity.EditMode;
    }
}
