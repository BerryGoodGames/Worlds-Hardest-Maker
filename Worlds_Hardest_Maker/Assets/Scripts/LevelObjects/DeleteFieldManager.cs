using UnityEngine;

public class DeleteFieldManager : ILevelObjectPlacer
{
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Delete;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 matrixPosition = request.Position.ConvertToMatrix();
        
        // remove player if at deleted pos
        PlayerManager.Instance.RemoveAtPosIntersectInSheet(matrixPosition, request.Sheet);
        
        // delete field
        bool deletedField = FieldManager.Instance.Remove(matrixPosition, true, request.Sheet);

        return new(deletedField, null);
    }
}