using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMode", menuName = "ScriptableObjects/EditMode/EntityMode")]
public class EntityMode : EditMode
{
    protected virtual void Reset()
    {
        WorldPositionType = WorldPositionType.Grid;
        IsDraggable = true;
    }
}