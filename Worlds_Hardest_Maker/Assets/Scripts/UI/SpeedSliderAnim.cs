using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSliderAnim : MonoBehaviour
{
    private RectTransform rt;
    private UIFollowEntity follow;
    private Animator anim;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        follow = GetComponent<UIFollowEntity>();
        anim = GetComponent<Animator>();
        
        Gone();
    }

    private void Update()
    {
        // set visible status (if no other slider is hovered)
        Animator anim = GetComponent<Animator>();
        bool hoveredHitbox = follow.entity.GetComponent<HoverSliderDetection>().MouseHoverSlider() && (!HoverSliderDetection.sliderHovered || anim.GetBool("Visible"));

        bool vis = !GameManager.Instance.Playing && Input.GetKey(GameManager.Instance.EditSpeedKey) && hoveredHitbox;

        if (anim.GetBool("Visible") && !vis) HoverSliderDetection.sliderHovered = false;

        anim.SetBool("Visible", vis);

        if (hoveredHitbox && vis) HoverSliderDetection.sliderHovered = true;
    }

    public void Gone()
    {
        if (!anim.GetBool("Visible"))
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