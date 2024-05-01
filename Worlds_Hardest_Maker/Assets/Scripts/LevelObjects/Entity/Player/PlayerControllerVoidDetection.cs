using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private void VoidDetection()
    {
        if (LevelSessionEditManager.Instance.Editing
            || InDeathAnim
            || CurrentPlatforms.Count > 0
            || !CheckVoidCollision(transform.position)) return;
        
        // check corners
        int collisionId = 0;
        
        List<Vector2> fallPositions = new();
        
        for (int x = -1; x < 2 && (CollisionCount() < 2 || collisionId == 3); x += 2)
        {
            for (int y = -1; y < 2 && (CollisionCount() < 2 || collisionId == 3); y += 2)
            {
                ParseCollisionLoop(x, y, ref collisionId, ref fallPositions);
            }
        }
        
        if (CollisionCount() >= 2 && collisionId != 3) DieVoid(FindClosestFallPosition(fallPositions));
        
        return;
        
        int CollisionCount() => fallPositions.Count;
    }
    
    /// <param name="position">position where to check</param>
    /// <returns>if there is a void at the position</returns>
    private static bool CheckVoidCollision(Vector2 position) =>
        // return Physics2D.OverlapPoint(position, LayerManager.Instance.Layers.Void) != null;
        Physics2D.OverlapCircle(position, 0.05f, LayerManager.Instance.Layers.Void);
    
    private void ParseCollisionLoop(int x, int y, ref int collisionId, ref List<Vector2> fallPositions)
    {
        Transform t = transform;
        Vector3 playerScale = t.lossyScale;
        
        Vector2 checkRelativePosition = new(playerScale.x * x * 0.5f, playerScale.y * y * 0.5f);
        Vector2 position = checkRelativePosition + (Vector2)t.position;
        
        if (!CheckVoidCollision(position)) return;
        
        collisionId += (int)(Mathf.Clamp01(x) + Mathf.Clamp01(y) * 2);
        fallPositions.Add(position.ConvertToMatrix());
    }
    
    private Vector2 FindClosestFallPosition(List<Vector2> positions)
    {
        Vector2 fallPosition = positions[0];
        float currentMinDist = Vector2.Distance(transform.position, fallPosition);
        
        for (int i = 1; i < positions.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, positions[i]);
            
            if (!(dist < currentMinDist)) continue;
            
            currentMinDist = dist;
            fallPosition = positions[i];
        }
        
        return fallPosition;
    }
}