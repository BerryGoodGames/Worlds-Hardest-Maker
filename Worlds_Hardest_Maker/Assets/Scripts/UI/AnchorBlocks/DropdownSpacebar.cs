using MyBox;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls wether the dropdown opens when the user presses space
/// </summary>
public class DropdownSpacebar : MonoBehaviour
{
    [SerializeField] [InitializationField] [OverrideLabel("Open on spacebar press")]
    private bool open = true;
    [Separator]
    [SerializeField] [InitializationField] [MustBeAssigned]
    private TMP_Dropdown dropdown;

    private void Update()
    {
        // prevent dropdown from opening when user presses space
        if (!open && dropdown && dropdown.IsExpanded && Input.GetKeyDown(KeyCode.Space)) dropdown.Hide();
    }
}
