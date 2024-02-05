using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance { get; private set; }

    #region Setting UI element references
    
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput coinsNeededInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle isCoinsNeededLimitedInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput drownDurationInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Slider waterDampingSlider;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput iceFrictionInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput iceMaxSpeedInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle reusableCheckpointCheckbox;

    #endregion

    #region Setting variables
    
    [HideInInspector] public bool IsCoinsNeededLimited;
    
    [HideInInspector] public int CoinsNeeded;

    [HideInInspector] public float DrownDuration;

    [HideInInspector] public float WaterDampingFactor;

    [HideInInspector] public float IceFriction;

    [HideInInspector] public float IceMaxSpeed;

    public bool ReusableCheckpoints
    {
        get => CheckpointController.ReusableCheckpoints;
        set => CheckpointController.ReusableCheckpoints = value;
    }

    #endregion


    #region Level settings
    
    public void SetCoinsNeeded() => CoinsNeeded = (int)coinsNeededInput.GetCurrentNumber();
    public void SetCoinsNeeded(int value)
    {
        CoinsNeeded = value;
        coinsNeededInput.SetNumberText(value);
    }

    public void SetIsNeededCoinsLimited() => IsCoinsNeededLimited = isCoinsNeededLimitedInput.isOn;
    public void SetIsNeededCoinsLimited(bool value)
    {
        IsCoinsNeededLimited = value;
        isCoinsNeededLimitedInput.isOn = IsCoinsNeededLimited;
    }

    public void SetDrownDuration() => DrownDuration = drownDurationInput.GetCurrentNumber();

    public void SetDrownDuration(float drownDuration)
    {
        DrownDuration = drownDuration;
        drownDurationInput.SetNumberText(drownDuration);
    }

    public void SetWaterDamping() => WaterDampingFactor = 1 - waterDampingSlider.value;
    public void SetWaterDamping(float waterDamping)
    {
        WaterDampingFactor = waterDamping;
        waterDampingSlider.value = 1 - waterDamping;
    }

    public void SetIceFriction() => IceFriction = iceFrictionInput.GetCurrentNumber();
    public void SetIceFriction(float friction)
    {
        IceFriction = friction;
        iceFrictionInput.SetNumberText(friction);
    }

    public void SetIceMaxSpeed() => IceMaxSpeed = iceMaxSpeedInput.GetCurrentNumber();
    public void SetIceMaxSpeed(float speed)
    {
        IceMaxSpeed = speed;
        iceMaxSpeedInput.SetNumberText(speed);
    }

    public void SetReusableCheckpoints() => ReusableCheckpoints = reusableCheckpointCheckbox.isOn;
    public void SetReusableCheckpoints(bool reusableCheckpoint)
    {
        ReusableCheckpoints = reusableCheckpoint;
        reusableCheckpointCheckbox.isOn = reusableCheckpoint;
    }

    #endregion

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetCoinsNeeded();
        SetIsNeededCoinsLimited();
        SetDrownDuration();
        SetIceFriction();
        SetIceMaxSpeed();
        SetWaterDamping();
    }
}