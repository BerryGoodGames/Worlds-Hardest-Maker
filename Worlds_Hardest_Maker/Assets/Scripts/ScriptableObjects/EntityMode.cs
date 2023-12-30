using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMode", menuName = "ScriptableObjects/EntityMode")]
public class EntityMode : EditMode
{
    protected virtual void Reset()
    {
        Attributes.IsEntity = true;
        WorldPositionType = WorldPositionType.Grid;
        IsDraggable = true;
    }
}