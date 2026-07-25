using UnityEngine;

/// <summary>
///     Service responsible for calculating and applying preview rotation
///     based on edit mode and configuration.
/// </summary>
public interface IPreviewRotationService
{
    /// <summary>
    ///     Calculates the target rotation for preview display.
    /// </summary>
    Quaternion GetTargetRotation(bool shouldRotate);
}
