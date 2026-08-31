using UnityEngine;
using VContainer;

public class DeleteFieldManager : ILevelObjectPlacer
{
    [Inject] private IFieldManager fieldManager;
    [Inject] private IPlayerManager playerManager;
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Delete;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 matrixPosition = request.Position.ConvertToMatrix();
        
        // remove player if at deleted pos
        playerManager.RemoveAtPosIntersectInSheet(matrixPosition, request.Sheet);
        
        // delete field
        bool deletedField = fieldManager.Remove(matrixPosition, request.Sheet, true);

        return new(deletedField, null);
    }
}