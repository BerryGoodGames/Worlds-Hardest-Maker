using UnityEngine;

public class DeletionManager : ILevelObjectManager
{
    public bool CanHandle(EditMode editMode)
    {
        throw new System.NotImplementedException();
    }

    public LevelObjectController Place(PlacementRequest request)
    {
        throw new System.NotImplementedException();
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }
}