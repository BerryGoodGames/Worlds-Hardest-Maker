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
    #endregion

    public LevelSettingsData(LevelSettings settings)
    {
        // fetch variables
        drownDuration = settings.drownDuration;
        waterDamping = settings.waterDamping;
        iceFriction = settings.iceFriction;
        iceMaxSpeed = settings.iceMaxSpeed;
    }

    public override void ImportToLevel()
    {
        Debug.Log($"import, {drownDuration}, {waterDamping}");
        LevelSettings.Instance.SetDrownDuration(drownDuration, false);
        LevelSettings.Instance.SetIceFriction(iceFriction, false);
        LevelSettings.Instance.SetIceMaxSpeed(iceMaxSpeed, false);
        LevelSettings.Instance.SetWaterDamping(waterDamping, false);
        LevelSettings.Instance.SyncPlayersToSettings();
    }
}
