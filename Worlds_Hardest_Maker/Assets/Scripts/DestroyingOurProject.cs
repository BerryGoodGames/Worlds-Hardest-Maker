using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// fills wall fields centered around (0, 0), amount based on INTENSITY
/// attach to new gameObject
/// </summary>
public class DestroyingOurProject : MonoBehaviour
{
    public int INTENSITY;

    private void Start()
    {
        print($"We're about to fill {Mathf.Pow(INTENSITY * 2 + 1, 2)} fields! (gotta go)");

        SelectionManager.Instance.FillArea(new(-INTENSITY, -INTENSITY), new(INTENSITY, INTENSITY), FieldType.WALL_FIELD);
        SaveSystem.SaveCurrentLevel();
    }
}
