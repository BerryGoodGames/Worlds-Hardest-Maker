using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// contains methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
/// attach to game manager
/// </summary>
public class FillManager : MonoBehaviour
{
    private Vector2 prevStart;
    private Vector2 prevEnd;
    private void Update()
    {
        // update fill markings
        if (!GameManager.Instance.Playing && GameManager.Instance.MouseDragStart != null && GameManager.Instance.MouseDragEnd != null && GameManager.Instance.Filling)
        {
            Vector2 start = (Vector2)GameManager.Instance.MouseDragStart;
            Vector2 end = (Vector2)GameManager.Instance.MouseDragEnd;
            // disable normal placement preview
            GameManager.Instance.PlacementPreview.SetActive(false);

            if (prevStart == null || !prevStart.Equals(start) || !prevEnd.Equals(end))
            {
                // reset fill marking
                foreach (Transform stroke in GameManager.Instance.FillOutlineContainer.transform)
                {
                    Destroy(stroke.gameObject);
                }
                foreach (Transform preview in GameManager.Instance.FillPreviewContainer.transform)
                {
                    Destroy(preview.gameObject);
                }
                // print($"Filling from {start} to {end}");

                List<Vector2> fillRange = GetFillRange(start, end);

                LineManager.SetWeight(0.1f);
                LineManager.SetFill(Color.black);

                float width = end.x - start.x;
                float height = end.y - start.y;

                LineManager.DrawRect(
                    width > 0 ? start.x - 0.5f : start.x + 0.5f, 
                    height > 0 ? start.y - 0.5f : start.y + 0.5f, 
                    width > 0 ? width + 1 : width - 1, 
                    height > 0? height + 1 : height - 1, 
                    1000, false, GameManager.Instance.FillOutlineContainer.transform
                );

                foreach (Vector2 pos in fillRange)
                {
                    Instantiate(GameManager.Instance.FillPreview, pos, Quaternion.identity, GameManager.Instance.FillPreviewContainer.transform);
                }

                GameManager.Instance.CurrentFillRange = fillRange;
            }
            prevStart = (Vector2)GameManager.Instance.MouseDragStart;
            prevEnd = (Vector2)GameManager.Instance.MouseDragEnd;
        }
    }

    // get bounds of multiple points (in matrix)
    private static (float, float, float, float) GetBounds(List<Vector2> points)
    {
        float lowestX = points[0].x;
        float highestX = points[0].x;
        float lowestY = points[0].y;
        float highestY = points[0].y;
        foreach (Vector2 pos in points)
        {
            if (lowestX > pos.x) lowestX = (int)pos.x;
            if (lowestY > pos.y) lowestY = (int)pos.y;
            if (highestX < pos.x) highestX = (int)pos.x;
            if (highestY < pos.y) highestY = (int)pos.y;
        }
        return (lowestX, highestX, lowestY, highestY);
    }
    private static (float, float, float, float) GetBounds(params Vector2[] points) { return GetBounds(points.ToList()); }
    private static (int, int, int, int) GetBoundsMatrix(List<Vector2> points)
    {
        var (lowestX, highestX, lowestY, highestY) = GetBounds(points);
        return ((int)lowestX, (int)highestX, (int)lowestY, (int)highestY);
    }
    private static (int, int, int, int) GetBoundsMatrix(params Vector2[] points) { return GetBoundsMatrix(points.ToList()); }

    public static List<Vector2> GetFillRange(Vector2 p1, Vector2 p2)
    {
        // find bounds
        var (lowestX, highestX, lowestY, highestY) = GetBoundsMatrix(p1, p2);

        // collect every pos in range
        List<Vector2> res = new();
        for (int x = lowestX; x <= highestX; x++)
        {
            for (int y = lowestY; y <= highestY; y++)
            {
                res.Add(new(x, y));
            }
        }
        return res;
    }

    
    public static void FillArea(List<Vector2> poses, FieldManager.FieldType type)
    {
        if (GameManager.Instance.CurrentFillRange == null) return;
        GameManager.Instance.CurrentFillRange = null;

        // find bounds
        var (lowestX, highestX, lowestY, highestY) = GetBoundsMatrix(poses);

        // check if its 1 wide
        if (lowestX == highestX || lowestY == highestY)
        {
            foreach(Vector2 pos in poses)
            {
                FieldManager.SetField((int)pos.x, (int)pos.y, type);
            }
            return;
        }
        // clear area
        foreach (Vector2 pos in poses)
        {
            FieldManager.RemoveField((int)pos.x, (int)pos.y);
        }
        foreach (Vector2 pos in poses)
        {
            int mx = (int)pos.x;
            int my = (int)pos.y;

            // set field at pos
            GameObject prefab = FieldManager.GetPrefabByType(type);
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity, GameManager.Instance.FieldContainer.transform);
            if(obj.TryGetComponent(out FieldOutline FOComp))
            {
                FOComp.updateOnStart = false;
            }
            // remove player if at changed pos
            if (type != FieldManager.FieldType.START_FIELD)
            {
                PlayerManager.RemovePlayerAtPos(mx, my);
            }

            if (type == FieldManager.FieldType.WALL_FIELD)
            {
                // remove coin if wall is placed
                GameManager.RemoveObjectInContainer(mx, my, GameManager.Instance.CoinContainer);
            }
        }

        // update outlines
        if (FieldManager.GetPrefabByType(type).GetComponent<FieldOutline>() != null)
        {
            FieldOutline FOComp;
            for (int i = lowestX; i <= highestX; i++)
            {
                if (FieldManager.GetField(i, lowestY).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.down, true);

                if (FieldManager.GetField(i, highestY).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.up, true);
            }

            for (int i = lowestY; i <= highestY; i++)
            {
                if (FieldManager.GetField(lowestX, i).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.left, true);

                if (FieldManager.GetField(highestX, i).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.right, true);
            }
        }
    }
    public static void FillArea(Vector2 start, Vector2 end, FieldManager.FieldType type)
    {
        FillArea(GetFillRange(start, end), type);
    }
}
