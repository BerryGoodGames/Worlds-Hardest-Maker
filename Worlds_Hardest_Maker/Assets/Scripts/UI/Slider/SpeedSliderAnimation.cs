using UnityEngine;

public class SpeedSliderAnim : MonoBehaviour
{
    private RectTransform rt;
    private UIFollowEntity follow;
    private SpeedSliderTween anim;

    private HoverSliderDetection hoverSliderDetection;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        follow = GetComponent<UIFollowEntity>();
        anim = GetComponent<SpeedSliderTween>();

        hoverSliderDetection = follow.Entity.GetComponent<HoverSliderDetection>();

        Gone();

        if (follow != null && follow.Entity != null) return;

        Destroy(gameObject);
    }

    private void Update()
    {
        if (follow.Entity == null)
        {
            Destroy(gameObject);
            return;
        }

        // set visible status (if no other slider is hovered)
        bool hoveredHitbox = hoverSliderDetection.MouseHoverSlider() &&
                             (!HoverSliderDetection.SliderHovered || anim.IsVisible());

        bool visible = !EditModeManagerOther.Instance.Playing && KeyBinds.GetKeyBind("Editor_Modify") &&
                       hoveredHitbox;

        if (!visible && anim.IsVisible()) HoverSliderDetection.SliderHovered = false;

        anim.SetVisible(visible);

        if (visible) HoverSliderDetection.SliderHovered = true;
    }

    public void Gone()
    {
        if (!follow) return;

        if (anim.IsVisible()) return;

        follow.enabled = false;
        rt.position = new(2000, 2000);
    }

    public void Ungone() => follow.enabled = true;
}