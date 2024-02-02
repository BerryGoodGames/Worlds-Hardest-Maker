using UnityEngine;

/// <summary>
///     Checks if optionbar should be visible
///     <para>Attach to hovering hitbox of optionbar</para>
/// </summary>
public class ToolOptionbarHoveringHitbox : MonoBehaviour
{
    [SerializeField] private ToolOptionbar toolOptionbar;

    private AlphaTween anim;
    private MouseOverUIRect mo;

    private void Start()
    {
        mo = GetComponent<MouseOverUIRect>();
        anim = toolOptionbar.GetComponent<AlphaTween>();
    }

    private void Update() =>
        anim.SetVisible(
            (mo.Over || toolOptionbar.Root.MouseOverUIRect.Over)
            && (anim.IsVisible || toolOptionbar.Root.MouseOverUIRect.Over)
            && !LevelSessionEditManager.Instance.Playing
            && !ReferenceManager.Instance.Menu.activeSelf
        );
}