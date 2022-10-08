using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSettingsData : IData
{
    #region SETTING VARIABLES
    public float drownDuration;
    public float waterDamping;
    public float iceFriction;
    public float iceMaxSpeed;
    public bool reusableCheckpoints;
    #endregion

    public LevelSettingsData(LevelSettings settings)
    {
        // fetch variables
        drownDuration = settings.drownDuration;
        waterDamping = settings.waterDamping;
        iceFriction = settings.iceFriction;
        iceMaxSpeed = settings.iceMaxSpeed;
        reusableCheckpoints = settings.reusableCheckpoints;
    }

    public override void ImportToLevel()
    {
        LevelSettings.Instance.SetDrownDuration(drownDuration, false);
        LevelSettings.Instance.SetIceFriction(iceFriction, false);
        LevelSettings.Instance.SetIceMaxSpeed(iceMaxSpeed, false);
        LevelSettings.Instance.SetWaterDamping(waterDamping, false);
        LevelSettings.Instance.SetReusableCheckpoints(reusableCheckpoints, false);
        LevelSettings.Instance.SyncPlayersToSettings();
    }
}
