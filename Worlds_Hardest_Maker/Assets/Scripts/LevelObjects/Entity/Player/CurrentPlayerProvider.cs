public class CurrentPlayerProvider : IPlayerProvider
{
    public PlayerController Player { get; private set; }
    public bool HasPlayer { get; private set; }
    
    public void DropPlayer()
    {
        Player = null;
        HasPlayer = false;
    }

    public void SetPlayer(PlayerController player)
    {
        Player = player;
        HasPlayer = true;
    }
}