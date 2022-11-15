using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSliderAnim : MonoBehaviour
{
    private RectTransform rt;
    private UIFollowEntity follow;
    private SpeedSliderTween anim;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        follow = GetComponent<UIFollowEntity>();
        anim = GetComponent<SpeedSliderTween>();
        
        Gone();
    }

    private void Update()
    {
        if (follow == null || follow.entity == null) {
            Destroy(gameObject);
            return;
        }
        // set visible status (if no other slider is hovered)
        bool hoveredHitbox = follow.entity.GetComponent<HoverSliderDetection>().MouseHoverSlider() && (!HoverSliderDetection.sliderHovered || anim.IsVisible());

        bool vis = !GameManager.Instance.Playing && Input.GetKey(GameManager.Instance.EditSpeedKey) && hoveredHitbox;

        if (!vis && anim.IsVisible()) HoverSliderDetection.sliderHovered = false;

        anim.SetVisible(vis);

        if (vis && hoveredHitbox) HoverSliderDetection.sliderHovered = true;
    }

    public void Gone()
    {
        if (!anim.IsVisible())
        {
            follow.enabled = false;
            rt.position = new(2000, 2000);
        }
    }

    public void Ungone()
    {
        follow.enabled = true;
    }
}