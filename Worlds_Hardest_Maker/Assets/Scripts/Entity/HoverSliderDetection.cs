using System.Linq;
using UnityEngine;

/// <summary>
///     detects if one of hitboxes are colliding with mouse position
///     requires child named HoveringHitboxes containing hitboxes
///     attach to entity having speed slider
/// </summary>
public class HoverSliderDetection : MonoBehaviour
{
    public static bool sliderHovered = false;

    public GameObject[] roots;
    private bool hovered;

    public bool MouseHoverSlider()
    {
        foreach (Transform collider in transform.GetChild(0))
        {
            if (!collider.GetComponent<MouseOver>().Over) continue;

            if (roots.Contains(collider.gameObject))
            {
                hovered = true;
                return true;
            }

            if (hovered || roots.Length == 0)
            {
                return true;
            }
        }

        hovered = false;
        return false;
    }
}