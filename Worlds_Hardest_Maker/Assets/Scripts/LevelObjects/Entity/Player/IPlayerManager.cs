using UnityEngine;

public interface IPlayerManager
{
    public PlayerController CreateNew(Vector2 position, ISheet sheet, bool surroundWithStartFields);
    public PlayerController Set(Vector2 position);
    public void RemoveAtPos(Vector2 position);
    public void RemoveAtPosIntersectInSheet(Vector2 position, ISheet sheet);
    public Vector2Int GetCurrentRoom();
    public Vector2Int GetStartRoom();
}