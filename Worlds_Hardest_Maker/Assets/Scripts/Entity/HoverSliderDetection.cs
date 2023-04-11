using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Detects if one of hitboxes are colliding with mouse position
///     <para>Requires child named HoveringHitboxes containing hitboxes</para>
///     <para>Attach to entity having speed slider</para>
/// </summary>
public class HoverSliderDetection : MonoBehaviour
{
    public static bool SliderHovered = false;

    [FormerlySerializedAs("roots")] public GameObject[] Roots;
    private bool hovered;

    public bool MouseHoverSlider()
    {
        foreach (Transform collider in transform.GetChild(0))
        {
            if (!collider.GetComponent<MouseOver>().Over) continue;

            if (Roots.Contains(collider.gameObject))
            {
                hovered = true;
                return true;
            }

            if (hovered || Roots.Length == 0)
            {
                return true;
            }
        }

        hovered = false;
        return false;
    }
}