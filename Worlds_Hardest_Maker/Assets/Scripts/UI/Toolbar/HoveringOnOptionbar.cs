using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     checks if optionbar should be visible
///     attach to hovering hitbox of optionbar
/// </summary>
public class HoveringOnOptionbar : MonoBehaviour
{
    [FormerlySerializedAs("optionBar")] public GameObject OptionBar;
    private AlphaUITween anim;
    private MouseOverUI mo;

    private void Start()
    {
        mo = GetComponent<MouseOverUI>();
        anim = OptionBar.GetComponent<AlphaUITween>();
    }

    private void Update()
    {
        anim.SetVisible(mo.Over && !EditModeManager.Instance.Playing && !ReferenceManager.Instance.Menu.activeSelf);
    }
}