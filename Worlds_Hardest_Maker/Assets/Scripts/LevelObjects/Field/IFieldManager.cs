using System.Collections.Generic;
using UnityEngine;
using WorldsHardestMaker.Selection;

public interface IFieldManager
{
    public FieldController CreateNew(Vector2 position, int rotation, ISheet sheet, FieldMode fieldMode);
    public bool Remove(Vector2 position, ISheet sheet, bool updateOutlines = false);
    public void ApplySafeFieldsColor(bool oneColor);
    public void UpdateOutlinesInArea(bool hasOutline, SelectionArea area);
    public List<FieldController> GetNeighborsInSheet(GameObject gameObject, ISheet sheet);
}