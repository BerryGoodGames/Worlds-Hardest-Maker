using System;

[Serializable]
public class LevelSettingsData : Data
{
    #region Setting variables

    public int RoomWidth;
    public int RoomHeight;

    public float PlayerSpeed;

    public bool IsCoinsNeededLimited;
    public int CoinsNeeded;

    public bool PlayerInvincibility;

    public float DrownDuration;

    public float WaterDamping;
    public float IceFriction;
    public float IceMaxSpeed;

    public float ConveyorSpeed;

    public bool ReusableCheckpoints;

    #endregion

    public LevelSettingsData(LevelSettings settings)
    {
        // fetch variables
        RoomWidth = settings.RoomWidth;
        RoomHeight = settings.RoomHeight;
        PlayerSpeed = settings.PlayerSpeed;
        IsCoinsNeededLimited = settings.IsCoinsNeededLimited;
        CoinsNeeded = settings.CoinsNeeded;
        PlayerInvincibility = settings.PlayerInvincibility;
        DrownDuration = settings.DrownDuration;
        WaterDamping = settings.WaterDampingFactor;
        IceFriction = settings.IceFriction;
        IceMaxSpeed = settings.IceMaxSpeed;
        ConveyorSpeed = settings.ConveyorSpeed;
        ReusableCheckpoints = settings.ReusableCheckpoints;
    }

    public override void ImportToLevel()
    {
        LevelSettings.Instance.SetRoomWidth(RoomWidth);
        LevelSettings.Instance.SetRoomHeight(RoomHeight);
        LevelSettings.Instance.SetPlayerSpeed(PlayerSpeed);
        LevelSettings.Instance.SetIsNeededCoinsLimited(IsCoinsNeededLimited);
        LevelSettings.Instance.SetCoinsNeeded(CoinsNeeded);
        LevelSettings.Instance.SetPlayerInvincibility(PlayerInvincibility);
        LevelSettings.Instance.SetDrownDuration(DrownDuration);
        LevelSettings.Instance.SetWaterDamping(WaterDamping);
        LevelSettings.Instance.SetIceFriction(IceFriction);
        LevelSettings.Instance.SetIceMaxSpeed(IceMaxSpeed);
        LevelSettings.Instance.SetConveyorSpeed(ConveyorSpeed);
        LevelSettings.Instance.SetReusableCheckpoints(ReusableCheckpoints);

        LevelSettings.Instance.InvokeOnImported();
    }

    public override EditMode GetEditMode() => EditModeManager.Wall;

    public override bool Equals(Data d)
    {
        LevelSettingsData other = (LevelSettingsData)d;
        return other.RoomWidth == RoomWidth
               && other.RoomHeight == RoomHeight
               && other.PlayerSpeed == PlayerSpeed
               && other.IsCoinsNeededLimited == IsCoinsNeededLimited
               && other.CoinsNeeded == CoinsNeeded
               && other.PlayerInvincibility == PlayerInvincibility
               && other.DrownDuration == DrownDuration
               && other.WaterDamping == WaterDamping
               && other.IceFriction == IceFriction
               && other.IceMaxSpeed == IceMaxSpeed
               && other.ConveyorSpeed == ConveyorSpeed
               && other.ReusableCheckpoints == ReusableCheckpoints;
    }
}