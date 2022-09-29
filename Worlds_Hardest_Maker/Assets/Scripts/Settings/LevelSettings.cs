using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance { get; private set; }

    #region SETTING UI ELEMENT REFERENCES
    [SerializeField] private NumberInput drownDurationInput;
    [SerializeField] private Slider waterDampingSlider;
    [SerializeField] private NumberInput iceFrictionInput;
    [SerializeField] private NumberInput iceMaxSpeedInput;
    [SerializeField] private Toggle reusableCheckpointCheckbox;
    #endregion

    #region SETTING VARIABLES
    [HideInInspector] public float drownDuration;
    [HideInInspector] public float waterDamping;
    [HideInInspector] public float iceFriction;
    [HideInInspector] public float iceMaxSpeed;
    [HideInInspector] public bool reusableCheckpoints
    {
        get { return CheckpointController.ReusableCheckpoints; }
        set { CheckpointController.ReusableCheckpoints = value; }
    }
    #endregion

    #region LEVEL SETTINGS
    public void SetDrownDuration(bool syncPlayers = true)
    {
        Instance.drownDuration = drownDurationInput.GetCurrentNumber();
        if(syncPlayers) SyncPlayersToSettings();
    }
    public void SetDrownDuration(float drownDuration, bool syncPlayers = true)
    {
        Instance.drownDuration = drownDuration;
        drownDurationInput.SetNumberText(drownDuration);
        if (syncPlayers) SyncPlayersToSettings();
    }

    public void SetWaterDamping(bool syncPlayers = true)
    {
        Instance.waterDamping = 1 - waterDampingSlider.value;
        if (syncPlayers) SyncPlayersToSettings();
    }
    public void SetWaterDamping(float waterDamping, bool syncPlayers = true)
    {
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
        Instance.reusableCheckpoints = reusableCheckpoint;
        reusableCheckpointCheckbox.isOn = reusableCheckpoint;
        if (syncPlayers) SyncPlayersToSettings();
    }
    public void SetReusableCheckpoints(bool syncPlayers = true)
    {
        Instance.reusableCheckpoints = reusableCheckpointCheckbox.isOn;
        reusableCheckpointCheckbox.isOn = reusableCheckpointCheckbox.isOn;
        if (syncPlayers) SyncPlayersToSettings();
    }
    #endregion

    public void SyncPlayersToSettings()
    {
        PlayerController[] controllers = FindObjectsOfType<PlayerController>();
        foreach(PlayerController p in controllers)
        {
            p.SyncToLevelSettings();
        }
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
