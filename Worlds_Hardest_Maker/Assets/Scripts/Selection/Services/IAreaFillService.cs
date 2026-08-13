using UnityEngine;

public interface IAreaFillService
{
    public void FillAreaWithFields(SelectionArea area, FieldMode mode);

    public void FillArea(SelectionArea area, EditMode editMode);
    
    public void FillArea(Vector2 start, Vector2 end, EditMode editMode) => FillArea(SelectionGeometry.GetFillArea(start, end), editMode);

    public void AdaptAreaToFieldType(SelectionArea area, FieldMode mode);
}