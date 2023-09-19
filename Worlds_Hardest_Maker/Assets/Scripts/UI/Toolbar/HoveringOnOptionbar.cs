using UnityEngine;

/// <summary>
///     Checks if optionbar should be visible
///     <para>Attach to hovering hitbox of optionbar</para>
/// </summary>
public class HoveringOnOptionbar : MonoBehaviour
{
    public GameObject OptionBar;
    private AlphaTween anim;
    private MouseOverUIRect mo;

    private void Start()
    {
        mo = GetComponent<MouseOverUIRect>();
        anim = OptionBar.GetComponent<AlphaTween>();
    }

    private void Update() =>
        anim.SetVisible(mo.Over && !EditModeManager.Instance.Playing &&
                        !ReferenceManager.Instance.Menu.activeSelf);
}