using UnityEngine;

public class DeleteFieldManager : ILevelObjectManager
{
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Delete;
    }

    public bool Place(PlacementRequest request)
    {
        Vector2 matrixPosition = request.Position.ConvertToMatrix();
        
        // remove player if at deleted pos
        PlayerManager.Instance.RemoveAtPosIntersectInSheet(matrixPosition, request.Sheet);
        
        // delete field
        bool deletedField = FieldManager.Instance.Remove(matrixPosition, true, request.Sheet);

        return deletedField;
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