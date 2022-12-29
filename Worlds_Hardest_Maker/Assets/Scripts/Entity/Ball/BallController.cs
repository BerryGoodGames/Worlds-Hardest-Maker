using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     parent class of every ball controller
/// </summary>
public abstract class BallController : Controller
{
    [HideInInspector] public float speed;
    [HideInInspector] public AppendSlider sliderController;
    [HideInInspector] public PhotonView photonView;

    private Text speedText;

    public void Awake()
    {
        sliderController = GetComponent<AppendSlider>();
        photonView = GetComponent<PhotonView>();

        // slider follow settings
        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.entity = gameObject;
        follow.offset = new(0, 0.5f);

        // slider init
        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener(value =>
        {
            float newSpeed = value * sliderController.Step;

            speed = newSpeed;

            if (MultiplayerManager.Instance.Multiplayer) photonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
        });
    }

    private void Start()
    {
        speedText = sliderController.GetSliderObject().transform.GetChild(0).GetComponent<Text>();
    }

    [PunRPC]
    public void SetSpeed(float speed)
    {
        this.speed = speed;

        // sync slider
        float currentSliderValue = sliderController.GetValue() / sliderController.Step;
        if (!Utils.DoFloatsEqual(currentSliderValue, speed))
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
        speedText.text = "Speed: " + speed.ToString("0.0");
    }

    [PunRPC]
    public abstract void MoveObject(Vector2 unitPos, int id);
}