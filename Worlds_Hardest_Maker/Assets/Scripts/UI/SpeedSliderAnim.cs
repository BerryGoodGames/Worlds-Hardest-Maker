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

        hoverSliderDetection = follow.entity.GetComponent<HoverSliderDetection>();

        Gone();

        if (follow != null && follow.entity != null) return;

        Destroy(gameObject);
    }

    private void Update()
    {
        // set visible status (if no other slider is hovered)
        bool hoveredHitbox = hoverSliderDetection.MouseHoverSlider() &&
                             (!HoverSliderDetection.sliderHovered || anim.IsVisible());

        bool vis = !EditModeManager.Instance.Playing && Input.GetKey(KeybindManager.Instance.editSpeedKey) &&
                   hoveredHitbox;

        if (!vis && anim.IsVisible()) HoverSliderDetection.sliderHovered = false;

        anim.SetVisible(vis);

        if (vis) HoverSliderDetection.sliderHovered = true;
    }

    public void Gone()
    {
        if (anim.IsVisible()) return;

        follow.enabled = false;
        rt.position = new(2000, 2000);
    }

    public void Ungone()
    {
        follow.enabled = true;
    }
}