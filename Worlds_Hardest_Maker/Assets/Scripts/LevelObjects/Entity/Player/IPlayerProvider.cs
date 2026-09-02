public interface IPlayerProvider
{
    public PlayerController Player { get; }
    public bool HasPlayer { get; }

    public void DropPlayer();
    public void SetPlayer(PlayerController player);
}