using UnityEngine;

public interface IAreaFillService
{
    public void FillAreaWithFields(SelectionArea area, FieldMode mode, Transform fieldContainer, Transform playerContainer);

    public void FillArea(SelectionArea area, EditMode editMode, Transform fieldContainer, Transform playerContainer);
    
    public void FillArea(Vector2 start, Vector2 end, EditMode editMode, Transform fieldContainer, Transform playerContainer)
    {
        FillArea(SelectionGeometry.GetFillArea(start, end), editMode, fieldContainer, playerContainer);
    }

    public void AdaptAreaToFieldType(SelectionArea area, FieldMode mode);
}