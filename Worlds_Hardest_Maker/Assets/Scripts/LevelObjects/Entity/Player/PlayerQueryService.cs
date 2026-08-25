using UnityEngine;

public class PlayerQueryService : ILevelObjectQuery<PlayerController>
{
    public PlayerController Find(Vector2 position, AnchorController sheet)
    {
        return Exists(position, sheet) ? PlayerManager.Instance.Player : null;
    }

    public bool Exists(Vector2 position, AnchorController sheet)
    {
        PlayerController player = PlayerManager.Instance.Player;
        if (player == null) return false;
        if (player.Sheet != sheet) return false;
        if ((Vector2)player.transform.position != position) return false;
        return true;
    }
}