using System;
using UnityEngine;

[Serializable]
public class LevelSettingsData : Data
{
    #region Setting variables

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
        reusableCheckpoints = settings.ReusableCheckpoints;
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

    public override void ImportToLevel(Vector2 pos)
    {
        ImportToLevel();
    }

    public override EditMode GetEditMode()
    {
        return EditMode.WALL_FIELD;
    }
}