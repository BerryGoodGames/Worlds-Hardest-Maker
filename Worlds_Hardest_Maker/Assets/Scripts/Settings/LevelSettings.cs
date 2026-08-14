using System;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance { get; private set; }
    
    public event Action OnLevelSettingsImported = () => { };
    public event Action OnUpdateRoomSize = () => { };
    
    
    #region Setting UI element references
    
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput roomWidthInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput roomHeightInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Slider playerSpeedInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput coinsNeededInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle isCoinsNeededLimitedInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle playerInvincibilityInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput drownDurationInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Slider waterDampingSlider;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput iceFrictionInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput iceMaxSpeedInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Slider conveyorSpeedInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle reusableCheckpointCheckbox;
    
    #endregion
    
    #region Setting variables
    
    [HideInInspector] public int RoomWidth;
    
    [HideInInspector] public int RoomHeight;
    
    [HideInInspector] public float PlayerSpeed;
    
    [HideInInspector] public bool IsCoinsNeededLimited;
    
    [HideInInspector] public int CoinsNeeded;
    
    [HideInInspector] public bool PlayerInvincibility;
    
    [HideInInspector] public float DrownDuration;
    
    [HideInInspector] public float WaterDampingFactor;
    
    [HideInInspector] public float IceFriction;
    
    [HideInInspector] public float IceMaxSpeed;
    
    [HideInInspector] public float ConveyorSpeed;
    
    public bool ReusableCheckpoints
    {
        get => CheckpointController.ReusableCheckpoints;
        set => CheckpointController.ReusableCheckpoints = value;
    }
    
    #endregion
    
    
    #region Level settings
    
    public void SetRoomWidth() => RoomWidth = (int)roomWidthInput.GetCurrentNumber();
    
    public void SetRoomWidth(int value)
    {
        RoomWidth = value;
        roomWidthInput.SetNumberText(value);
    }
    
    public void SetRoomHeight() => RoomHeight = (int)roomHeightInput.GetCurrentNumber();
    
    public void SetRoomHeight(int value)
    {
        RoomHeight = value;
        roomHeightInput.SetNumberText(value);
    }
    
    public void SetPlayerSpeed() => PlayerSpeed = playerSpeedInput.value / 2;
    
    public void SetPlayerSpeed(float value)
    {
        PlayerSpeed = value;
        
        if (playerSpeedInput == null) return;
        playerSpeedInput.value = (int)(value * 2);
    }
    
    public void SetCoinsNeeded() => CoinsNeeded = (int)coinsNeededInput.GetCurrentNumber();
    
    public void SetCoinsNeeded(int value)
    {
        CoinsNeeded = value;
        
        if (coinsNeededInput == null) return;
        coinsNeededInput.SetNumberText(value);
    }
    
    public void SetIsNeededCoinsLimited() => IsCoinsNeededLimited = isCoinsNeededLimitedInput.isOn;
    
    public void SetIsNeededCoinsLimited(bool value)
    {
        IsCoinsNeededLimited = value;
        
        if (isCoinsNeededLimitedInput == null) return;
        isCoinsNeededLimitedInput.isOn = IsCoinsNeededLimited;
    }
    
    public void SetPlayerInvincibility() => PlayerInvincibility = playerInvincibilityInput.isOn;
    
    public void SetPlayerInvincibility(bool value)
    {
        PlayerInvincibility = value;
        
        if (playerInvincibilityInput == null) return;
        playerInvincibilityInput.isOn = value;
    }
    
    public void SetDrownDuration() => DrownDuration = drownDurationInput.GetCurrentNumber();
    
    public void SetDrownDuration(float drownDuration)
    {
        DrownDuration = drownDuration;
        
        if (drownDurationInput == null) return;
        drownDurationInput.SetNumberText(drownDuration);
    }
    
    public void SetWaterDamping() => WaterDampingFactor = 1 - waterDampingSlider.value;
    
    public void SetWaterDamping(float waterDamping)
    {
        WaterDampingFactor = waterDamping;
        
        if (waterDampingSlider == null) return;
        waterDampingSlider.value = 1 - waterDamping;
    }
    
    public void SetIceFriction() => IceFriction = iceFrictionInput.GetCurrentNumber();
    
    public void SetIceFriction(float friction)
    {
        IceFriction = friction;
        
        if (iceFrictionInput == null) return;
        iceFrictionInput.SetNumberText(friction);
    }
    
    public void SetIceMaxSpeed() => IceMaxSpeed = iceMaxSpeedInput.GetCurrentNumber();
    
    public void SetIceMaxSpeed(float speed)
    {
        IceMaxSpeed = speed;
        
        if (iceMaxSpeedInput == null) return;
        iceMaxSpeedInput.SetNumberText(speed);
    }
    
    public void SetConveyorSpeed() => ConveyorSpeed = conveyorSpeedInput.value / 2;
    
    public void SetConveyorSpeed(float value)
    {
        ConveyorSpeed = value;
        
        if (conveyorSpeedInput == null) return;
        conveyorSpeedInput.value = (int)(value * 2);
    }
    
    public void SetReusableCheckpoints() => ReusableCheckpoints = reusableCheckpointCheckbox.isOn;
    
    public void SetReusableCheckpoints(bool reusableCheckpoint)
    {
        ReusableCheckpoints = reusableCheckpoint;
        
        if (reusableCheckpointCheckbox == null) return;
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
        LevelSessionManager.Instance.OnLevelLoaded += ImportTransitionRoomSize;
        
        SetRoomWidth();
        SetRoomHeight();
        SetPlayerSpeed();
        SetCoinsNeeded();
        SetIsNeededCoinsLimited();
        SetPlayerInvincibility();
        SetDrownDuration();
        SetIceFriction();
        SetIceMaxSpeed();
        SetConveyorSpeed();
        SetWaterDamping();
    }
    
    private void ImportTransitionRoomSize()
    {
        if (LevelSessionManager.IsSessionFromEditor) return;
        
        Vector2Int transitionRoomSize = TransitionManager.Instance.RoomSize;
        if (transitionRoomSize.x != 0) SetRoomWidth(transitionRoomSize.x);
        if (transitionRoomSize.y != 0) SetRoomHeight(transitionRoomSize.y);
        
        InvokeOnUpdateRoomSize();
    }
    
    private void OnDestroy() => LevelSessionManager.Instance.OnLevelLoaded -= ImportTransitionRoomSize;
    
    private void Update()
    {
        if (RoomWidth <= 0) SetRoomWidth(23);
        if (RoomHeight <= 0) SetRoomHeight(15);
    }
    
    public void InvokeOnImported() => OnLevelSettingsImported.Invoke();
    public void InvokeOnUpdateRoomSize() => OnUpdateRoomSize.Invoke();
}