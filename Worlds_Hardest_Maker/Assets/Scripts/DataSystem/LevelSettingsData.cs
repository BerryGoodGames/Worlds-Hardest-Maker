using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSettingsData : IData
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
        DrownDuration = settings.drownDuration;
        WaterDamping = settings.waterDamping;
        IceFriction = settings.iceFriction;
        IceMaxSpeed = settings.iceMaxSpeed;
        ReusableCheckpoints = settings.reusableCheckpoints;
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
