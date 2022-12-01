using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// parent class of every ball controller
/// </summary>
public abstract class IBallController : Controller
{
    [HideInInspector] public float speed;
    [HideInInspector] public AppendSlider sliderController;
    [HideInInspector] public PhotonView photonView;
    public void Awake()
    {
        // if(GameManager.Instance.Multiplayer) print("Init ball at: " + GetComponent<PhotonView>().Controller.NickName);

        sliderController = GetComponent<AppendSlider>();
        photonView = GetComponent<PhotonView>();
        
        // slider follow settings
        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.entity = gameObject;
        follow.offset = new(0, 0.5f);

        // slider init
        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener((value) =>
        {
            float newSpeed = value * sliderController.Step;

            speed = newSpeed;

            if (GameManager.Instance.Multiplayer) photonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
        });
    }

    [PunRPC]
    public void SetSpeed(float speed) { 
        this.speed = speed;

        // sync slider
        float currentSliderValue = sliderController.GetValue() / sliderController.Step;
        if(currentSliderValue != speed)
        {
            sliderController.GetSlider().SetValueWithoutNotify(speed / sliderController.Step);
        }
    }

    private void LateUpdate()
    {
        UpdateSpeedText();
    }

    public void UpdateSpeedText()
    {
        Text speedText = sliderController.GetSliderObject().transform.GetChild(0).GetComponent<Text>();
        speedText.text = "Speed: " + speed.ToString("0.0");
    }

    [PunRPC]
    public abstract void MoveObject(Vector2 unitPos, int id);
}