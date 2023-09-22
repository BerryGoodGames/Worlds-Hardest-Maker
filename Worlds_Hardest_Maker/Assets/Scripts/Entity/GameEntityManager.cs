using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEntityManager
{
    public static void RemoveEntitiesAt(Vector2 position)
    {
        const int entityLayer = 2 * 2 * 2 * 2 * 2 * 2 * 2; // 2 ^ 7

        Collider2D[] hits = Physics2D.OverlapPointAll(position, entityLayer);

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<EntityController>().Delete();
        }
    }
}
