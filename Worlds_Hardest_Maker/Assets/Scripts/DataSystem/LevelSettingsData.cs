using System;

[Serializable]
public class LevelSettingsData : Data
{
    #region Setting variables

    public bool IsCoinsNeededLimited;
    public int CoinsNeeded;

    public float DrownDuration;

    public float WaterDamping;
    public float IceFriction;
    public float IceMaxSpeed;

    public bool ReusableCheckpoints;

    #endregion

    public LevelSettingsData(LevelSettings settings)
    {
        // fetch variables
        IsCoinsNeededLimited = settings.IsCoinsNeededLimited;
        CoinsNeeded = settings.CoinsNeeded;
        DrownDuration = settings.DrownDuration;
        WaterDamping = settings.WaterDampingFactor;
        IceFriction = settings.IceFriction;
        IceMaxSpeed = settings.IceMaxSpeed;
        ReusableCheckpoints = settings.ReusableCheckpoints;
    }

    public override void ImportToLevel()
    {
        LevelSettings.Instance.SetIsNeededCoinsLimited(IsCoinsNeededLimited);
        LevelSettings.Instance.SetCoinsNeeded(CoinsNeeded);
        LevelSettings.Instance.SetDrownDuration(DrownDuration);
        LevelSettings.Instance.SetIceFriction(IceFriction);
        LevelSettings.Instance.SetIceMaxSpeed(IceMaxSpeed);
        LevelSettings.Instance.SetWaterDamping(WaterDamping);
        LevelSettings.Instance.SetReusableCheckpoints(ReusableCheckpoints);
    }

    public override EditMode GetEditMode() => EditModeManager.Wall;
    
    public override bool Equals(Data d)
    {
        LevelSettingsData other = (LevelSettingsData)d;
        return other.IsCoinsNeededLimited == IsCoinsNeededLimited
               && other.CoinsNeeded == CoinsNeeded 
               && other.DrownDuration == DrownDuration
               && other.IceFriction == IceFriction
               && other.IceMaxSpeed == IceMaxSpeed
               && other.WaterDamping == WaterDamping
               && other.ReusableCheckpoints == ReusableCheckpoints;
    }
}