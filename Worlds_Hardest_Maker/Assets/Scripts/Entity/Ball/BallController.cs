using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     parent class of every ball controller
/// </summary>
public abstract class BallController : Controller
{
    [FormerlySerializedAs("speed")] [HideInInspector] public float Speed;
    [FormerlySerializedAs("sliderController")] [HideInInspector] public AppendSlider SliderController;
    [FormerlySerializedAs("photonView")] [HideInInspector] public PhotonView PhotonView;

    private Text speedText;

    public void Awake()
    {
        SliderController = GetComponent<AppendSlider>();
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

    private void Start()
    {
    }

    [PunRPC]
    public void SetSpeed(float speed)
    {
        this.Speed = speed;

        // sync slider
        float currentSliderValue = SliderController.GetValue() / SliderController.Step;
        if (!currentSliderValue.EqualsFloat(speed))
        {
            SliderController.GetSlider().SetValueWithoutNotify(speed / SliderController.Step);
        }
    }

    private void LateUpdate()
    {
        UpdateSpeedText();
    }

    public void UpdateSpeedText()
    {
        speedText = speedText != null
            ? speedText
            : SliderController.GetSliderObject().transform.GetChild(0).GetComponent<Text>();
        speedText.text = "Speed: " + Speed.ToString("0.0");
    }

    [PunRPC]
    public abstract void MoveObject(Vector2 unitPos, int id);
}