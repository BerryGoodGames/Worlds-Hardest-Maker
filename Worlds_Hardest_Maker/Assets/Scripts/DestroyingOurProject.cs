using UnityEngine;

/// <summary>
///     Fills wall fields centered around (0, 0), amount based on INTENSITY
///     <para>Attach to new gameObject</para>
/// </summary>
public class DestroyingOurProject : MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    public int INTENSITY;

    private void Start()
    {
        print($"We're about to fill {Mathf.Pow(INTENSITY * 2 + 1, 2)} fields! (gotta go)");

        SelectionManager.Instance.FillArea(
            new(-INTENSITY, -INTENSITY), new(INTENSITY, INTENSITY),
            EditMode.WallField
        );

        SaveSystem.SaveCurrentLevel();
    }
}