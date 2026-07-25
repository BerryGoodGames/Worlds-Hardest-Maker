using UnityEngine;
using VContainer;

/// <summary>
///     Implementation of preview rotation service.
///     Handles rotation calculations based on edit mode and settings.
/// </summary>
public class PreviewRotationService : IPreviewRotationService
{
    public Quaternion GetTargetRotation(bool shouldRotate)
    {
        if (!shouldRotate)
        {
            return Quaternion.identity;
        }

        float rotationAngle = LevelSessionEditManager.Instance.EditRotation;
        return Quaternion.Euler(0, 0, rotationAngle);
    }
}
