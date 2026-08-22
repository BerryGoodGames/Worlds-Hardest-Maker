using System.Collections.Generic;
using UnityEngine;

public sealed class AnchorChainDropTargetResolver
{
    public DropTarget Resolve(Vector2 pointerPosition, IReadOnlyList<Rect> blockRectsInOrder, Rect connectorRect)
    {
        if (blockRectsInOrder == null || blockRectsInOrder.Count == 0)
        {
            return DropTarget.AppendEnd();
        }

        for (int i = 0; i < blockRectsInOrder.Count; i++)
        {
            if (blockRectsInOrder[i].Contains(pointerPosition))
            {
                return DropTarget.InsertAfter(i);
            }
        }

        if (connectorRect.Contains(pointerPosition))
        {
            return DropTarget.AppendEnd();
        }

        return DropTarget.None;
    }
}