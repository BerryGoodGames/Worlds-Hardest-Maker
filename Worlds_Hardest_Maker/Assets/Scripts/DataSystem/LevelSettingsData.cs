using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LevelSettingsData : Data
{
    #region Setting variables

    public float DrownDuration;

    public float WaterDamping;
    public float IceFriction;
    public float IceMaxSpeed;

    public bool ReusableCheckpoints;

    #endregion

    public LevelSettingsData(LevelSettings settings)
    {
        // fetch variables
        DrownDuration = settings.DrownDuration;
        WaterDamping = settings.WaterDamping;
        IceFriction = settings.IceFriction;
        IceMaxSpeed = settings.IceMaxSpeed;
        ReusableCheckpoints = settings.ReusableCheckpoints;
    }

    public override void ImportToLevel()
    {
        LevelSettings.Instance.SetDrownDuration(DrownDuration, false);
        LevelSettings.Instance.SetIceFriction(IceFriction, false);
        LevelSettings.Instance.SetIceMaxSpeed(IceMaxSpeed, false);
        LevelSettings.Instance.SetWaterDamping(WaterDamping, false);
        LevelSettings.Instance.SetReusableCheckpoints(ReusableCheckpoints, false);
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