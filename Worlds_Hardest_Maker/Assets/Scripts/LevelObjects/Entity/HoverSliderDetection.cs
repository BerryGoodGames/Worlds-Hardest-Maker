using System;
using System.Linq;
using MyBox;
using UnityEngine;

/// <summary>
///     Detects if one of hitboxes are colliding with mouse position
///     <para>Requires child named HoveringHitboxes containing hitboxes</para>
///     <para>Attach to entity having speed slider</para>
/// </summary>
public class HoverSliderDetection : MonoBehaviour
{
    public static bool SliderHovered = false;

    [SerializeField] [InitializationField] [MustBeAssigned] private Transform hoveringHitboxContainer;

    [SerializeField] private GameObject[] roots;
    private bool hovered;

    private MouseOver[] hitboxes;

    public bool MouseHoverSlider()
    {
        try
        {
            hitboxes ??= hoveringHitboxContainer.GetComponentsInChildren<MouseOver>();
            foreach (MouseOver hitbox in hitboxes)
            {
                if (!hitbox.Over) continue;

                if (roots.Contains(hitbox.gameObject))
                {
                    hovered = true;
                    return true;
                }

                if (hovered || roots.Length == 0) return true;
            }

            hovered = false;
        }
        catch (Exception e)
        {
            print(e.Message);
            print(name);
        }

        return false;
    }
}