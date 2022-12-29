using UnityEngine;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance { get; private set; }

    #region Setting UI element references

    [SerializeField] private NumberInput drownDurationInput;
    [SerializeField] private Slider waterDampingSlider;
    [SerializeField] private NumberInput iceFrictionInput;
    [SerializeField] private NumberInput iceMaxSpeedInput;
    [SerializeField] private Toggle reusableCheckpointCheckbox;

    #endregion

    #region Setting variables

    [HideInInspector] public float drownDuration;
    [HideInInspector] public float waterDamping;
    [HideInInspector] public float iceFriction;
    [HideInInspector] public float iceMaxSpeed;

    public bool ReusableCheckpoints
    {
        get => CheckpointController.ReusableCheckpoints;
        set => CheckpointController.ReusableCheckpoints = value;
    }

    #endregion


    #region Level settings

    public void SetDrownDuration(bool syncPlayers = true)
    {
        Instance.drownDuration = drownDurationInput.GetCurrentNumber();
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetDrownDuration(float drownDuration, bool syncPlayers = true)
    {
        Instance.drownDuration = drownDuration;
        drownDurationInput.SetNumberText(drownDuration);
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetWaterDamping(bool syncPlayers = true)
    {
        if (Instance == null) return;

        Instance.waterDamping = 1 - waterDampingSlider.value;
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetWaterDamping(float waterDamping, bool syncPlayers = true)
    {
        if (Instance == null) return;

        Instance.waterDamping = waterDamping;
        waterDampingSlider.value = 1 - waterDamping;
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetIceFriction(bool syncPlayers = true)
    {
        Instance.iceFriction = iceFrictionInput.GetCurrentNumber();
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetIceFriction(float friction, bool syncPlayers = true)
    {
        Instance.iceFriction = friction;
        iceFrictionInput.SetNumberText(friction);
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetIceMaxSpeed(bool syncPlayers = true)
    {
        Instance.iceMaxSpeed = iceMaxSpeedInput.GetCurrentNumber();
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetIceMaxSpeed(float speed, bool syncPlayers = true)
    {
        Instance.iceMaxSpeed = speed;
        iceMaxSpeedInput.SetNumberText(speed);
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetReusableCheckpoints(bool reusableCheckpoint, bool syncPlayers = true)
    {
        Instance.ReusableCheckpoints = reusableCheckpoint;
        reusableCheckpointCheckbox.isOn = reusableCheckpoint;
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetReusableCheckpoints(bool syncPlayers = true)
    {
        Instance.ReusableCheckpoints = reusableCheckpointCheckbox.isOn;
        reusableCheckpointCheckbox.isOn = reusableCheckpointCheckbox.isOn;
        if (syncPlayers) SyncPlayersToSettings();
    }

    #endregion

    public void SyncPlayersToSettings()
    {
        foreach (Transform player in ReferenceManager.Instance.playerContainer)
        {
            PlayerController p = player.GetComponent<PlayerController>();
            p.SyncToLevelSettings();
        }
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}