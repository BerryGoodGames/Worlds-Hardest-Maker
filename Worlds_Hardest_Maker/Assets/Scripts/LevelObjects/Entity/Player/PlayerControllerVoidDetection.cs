using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private void VoidDetection()
    {
        if (CheckVoidDetection()) return;
        
        bool[][] groundedMatrix = InitGroundedMatrix();
        
        List<Vector2> fallPositions = new();
        
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++) { ParseMatrixPosition(x, y, ref groundedMatrix, ref fallPositions); }
        }
        
        bool compensatingCorners = (groundedMatrix[0][0] && groundedMatrix[2][2]) || (groundedMatrix[2][0] && groundedMatrix[0][2]);
        bool compensatingEdges = (groundedMatrix[0][1] && groundedMatrix[2][1]) || (groundedMatrix[1][0] && groundedMatrix[1][2]);
        
        if (!compensatingEdges && !compensatingCorners)
        {
            DieVoid(FindClosestFallPosition(fallPositions));
        }
    }
    
    private bool CheckVoidDetection() =>
        LevelSessionEditManager.Instance.Editing
        || InDeathAnim
        || IsStandingOnPlatform
        || !IsCollidingWithVoid(transform.position);
    
    private static bool[][] InitGroundedMatrix()
    {
        bool[][] result = new bool[3][];
        for (int i = 0; i < 3; i++) { result[i] = new bool[3]; }
        
        return result;
    }
    
    private void ParseMatrixPosition(int x, int y, ref bool[][] groundedMatrix, ref List<Vector2> fallPositions)
    {
        if (x == 1 && y == 1) return;
        
        Transform t = transform;
        Vector3 playerScale = t.lossyScale;
        
        Vector2 checkPosition = new Vector2((x - 1) * 0.5f * playerScale.x, (y - 1) * 0.5f * playerScale.y) + (Vector2)transform.position;
        
        bool isVoidThere = IsCollidingWithVoid(checkPosition);
        
        bool isPlatformThere = IsPlatformThere(checkPosition);
        
        groundedMatrix[y][x] = isPlatformThere || !isVoidThere;
        
        // add to fall positions if corner is over void
        if (!groundedMatrix[y][x] && x != 1 && y != 1) fallPositions.Add(checkPosition);
    }
    
    private static bool IsPlatformThere(Vector2 checkPosition)
    {
        bool isPlatformThere = false;
        Collider2D[] platformHits = Physics2D.OverlapCircleAll(checkPosition, 0.05f, LayerManager.Instance.Layers.Field);
        foreach (Collider2D platformHit in platformHits)
        {
            if (!platformHit.TryGetComponent(out FieldController controller))
            {
                Debug.LogWarning($"Could not find a field controller on the hit {platformHit.name} when checking for colliding platforms");
                continue;
            }
            
            if (controller.FieldMode.CarryPlayer)
            {
                isPlatformThere = true;
                break;
            }
        }
        
        return isPlatformThere;
    }
    
    /// <param name="position">position where to check</param>
    /// <returns>if there is a void at the position</returns>
    private static bool IsCollidingWithVoid(Vector2 position) => Physics2D.OverlapCircle(position, 0.05f, LayerManager.Instance.Layers.Void);
    
    private Vector2 FindClosestFallPosition(List<Vector2> positions)
    {
        for (int i = 0; i < positions.Count; i++) positions[i] = positions[i].ConvertToMatrix();
        
        Vector2 fallPosition = positions[0];
        float currentMinDist = Vector2.Distance(transform.position, fallPosition);
        
        for (int i = 1; i < positions.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, positions[i]);
            
            if (dist >= currentMinDist) continue;
            
            currentMinDist = dist;
            fallPosition = positions[i];
        }
        
        return fallPosition;
    }
}