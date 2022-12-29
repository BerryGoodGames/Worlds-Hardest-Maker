using UnityEngine;

/// <summary>
///     checks if optionbar should be visible
///     attach to hovering hitbox of optionbar
/// </summary>
public class HoveringOnOptionbar : MonoBehaviour
{
    public GameObject optionBar;
    private AlphaUITween anim;
    private MouseOverUI mo;

    private void Start()
    {
        mo = GetComponent<MouseOverUI>();
        anim = optionBar.GetComponent<AlphaUITween>();
    }

    private void Update()
    {
        anim.SetVisible(mo.over && !GameManager.Instance.Playing && !ReferenceManager.Instance.Menu.activeSelf);
    }
}