using UnityEngine;

public class PreviewRotationDataProvider
{
    public PreviewRotationData GetPreviewRotationData(EditMode editMode)
    {
        bool shouldRotate = editMode.IsRotatable;
        Quaternion rotation = GetTargetRotation(shouldRotate);
        
        return new PreviewRotationData
        {
            TargetRotation = rotation,
            ResetRotation = !editMode.IsRotatable,
        };
    }
    
    private static Quaternion GetTargetRotation(bool shouldRotate)
    {
        if (!shouldRotate)
        {
            return Quaternion.identity;
        }
        
        float rotationAngle = LevelSessionEditManager.Instance.EditRotation;
        return Quaternion.Euler(0, 0, rotationAngle);
    }
}