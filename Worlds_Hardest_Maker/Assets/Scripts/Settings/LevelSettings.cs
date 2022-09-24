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

    #endregion

    #region SETTING VARIABLES
    [HideInInspector] public float drownDuration;
    [HideInInspector] public float waterDamping;
    [HideInInspector] public float iceFriction;
    [HideInInspector] public float iceMaxSpeed;
    #endregion

    #region LEVEL SETTINGS
    public void SetDrownDuration()
    {
        Instance.drownDuration = drownDurationInput.GetCurrentNumber();
    }
    public void SetWaterDamping()
    {
        Instance.waterDamping = waterDampingSlider.value;
    }
    public void SetIceFriction()
    {
        Instance.iceFriction = iceFrictionInput.GetCurrentNumber();
    }
    public void SetIceMaxSpeed()
    {
        Instance.iceMaxSpeed = iceMaxSpeedInput.GetCurrentNumber();
    }
    #endregion

    private void SyncPlayersToSettings()
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
