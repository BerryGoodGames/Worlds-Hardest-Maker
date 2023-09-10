using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Base class of every ball controller
/// </summary>
public abstract class BallController : Controller
{
    [HideInInspector] public float Speed;

    [HideInInspector] public AppendSlider SliderController;

    [HideInInspector] public PhotonView PhotonView;

    private TMP_Text speedText;

    public void Awake()
    {
        SliderController = GetComponent<AppendSlider>();
        speedText = SliderController.GetSliderObject().transform.GetChild(1).GetComponent<TMP_Text>();
        PhotonView = GetComponent<PhotonView>();

        // slider follow settings
        UIFollowEntity follow = SliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.Entity = gameObject;
        follow.Offset = new(0, 0.5f);

        // slider init
        Slider slider = SliderController.GetSlider();
        slider.onValueChanged.AddListener(value =>
        {
            float newSpeed = value * SliderController.Step;

            Speed = newSpeed;

            if (MultiplayerManager.Instance.Multiplayer) PhotonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
        });
    }

    [PunRPC]
    public void SetSpeed(float speed)
    {
        Speed = speed;

        // sync slider
        float currentSliderValue = SliderController.GetValue() / SliderController.Step;
        if (!currentSliderValue.EqualsFloat(speed))
            SliderController.GetSlider().SetValueWithoutNotify(speed / SliderController.Step);
    }

    private void LateUpdate() => UpdateSpeedText();

    public void UpdateSpeedText() =>
        // speedText = speedText != null
        //     ? speedText
        //     : SliderController.GetSliderObject().transform.GetChild(1).GetComponent<TMP_Text>();
        speedText.text = Speed.ToString("0.0");

    [PunRPC]
    public abstract void MoveObject(Vector2 unitPos, int id);
}