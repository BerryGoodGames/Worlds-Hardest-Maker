using UnityEngine;

public static class GameEntityManager
{
    public static void RemoveEntitiesAt(Vector2 position, LayerMask entityLayer)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, entityLayer);

        foreach (Collider2D hit in hits) hit.GetComponent<EntityController>().Delete();
    }
}