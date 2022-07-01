using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class IBallController : MonoBehaviour
{
    public abstract float SpeedMin { get; }
    public abstract float SpeedMax { get; }
    public static readonly float SpeedSliderStep = 0.5f;

    [HideInInspector] public float speed;
    [HideInInspector] public AppendSlider sliderController;
    public void Awake()
    {
        sliderController = GetComponent<AppendSlider>();

        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.entity = gameObject;
        follow.offset = new(0, 0.5f);

        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener((value) =>
        {
            speed = value * SpeedSliderStep;
        });
        speed = sliderController.GetValue();
    }

    

    private void LateUpdate()
    {
        UpdateSpeedText();
    }

    //public GameObject GetSlider()
    //{
    //    return transform.GetChild(0).gameObject;
    //}
    //public GameObject GetSliderObject()
    //{
    //    return GetSlider().transform.GetChild(0).GetChild(0).gameObject;
    //}
    //public void UpdateSliderPos()
    //{
    //    Vector2 sliderUnitPos = new(transform.position.x, transform.position.y + 0.58f);

    //    GameObject slider = GetSlider();
    //    Vector2 pos = Camera.main.WorldToScreenPoint(sliderUnitPos);
    //    slider.transform.position = pos;
    //}

    public void UpdateSpeedText()
    {
        Text speedText = sliderController.GetSliderObject().transform.GetChild(0).GetComponent<Text>();
        speedText.text = "Speed: " + speed.ToString("0.0");
    }

    public virtual void MoveObject(Vector2 unitPos, GameObject movedObject)
    {
        movedObject.transform.position = unitPos;
    }
}
