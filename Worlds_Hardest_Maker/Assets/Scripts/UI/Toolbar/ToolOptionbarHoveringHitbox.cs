using UnityEngine;

/// <summary>
///     Checks if optionbar should be visible
///     <para>Attach to hovering hitbox of optionbar</para>
/// </summary>
public class ToolOptionbarHoveringHitbox : MonoBehaviour
{
    public RectTransform ToolOptionbar;
    private AlphaTween anim;
    private MouseOverUIRect mo;

    private void Start()
    {
        mo = GetComponent<MouseOverUIRect>();
        anim = ToolOptionbar.GetComponent<AlphaTween>();
    }

    private void Update()
    {
        anim.SetVisible(
            mo.Over && !EditModeManagerOther.Instance.Playing &&
            !ReferenceManager.Instance.Menu.activeSelf
        );
    }
}