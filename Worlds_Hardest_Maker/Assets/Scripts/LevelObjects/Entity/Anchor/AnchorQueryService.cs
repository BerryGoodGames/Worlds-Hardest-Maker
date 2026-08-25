using UnityEngine;

public class AnchorQueryService : ILevelObjectQuery<AnchorController>
{
    public AnchorController Find(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f);
        
        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor")) return hit.gameObject.GetComponent<AnchorController>();
        }
        
        return null;
    }

    public bool Exists(Vector2 position, AnchorController sheet)
    {
        return Find(position, sheet) != null;
    }
}