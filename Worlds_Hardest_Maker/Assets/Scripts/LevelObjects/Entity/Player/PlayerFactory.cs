using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerFactory
{
    private readonly IObjectResolver diContainer;
    
    private PlayerController playerPrefab;
    private JumpToEntity mainCameraJumper;
    private TimerController timerController;
    private Transform playerContainer;

    public PlayerFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }
    
    public void Initialize(PlayerController playerPrefab, JumpToEntity mainCameraJumper, TimerController timerController, Transform playerContainer)
    {
        this.playerPrefab = playerPrefab;
        this.mainCameraJumper = mainCameraJumper;
        this.timerController = timerController;
        this.playerContainer = playerContainer;
    }
    
    public PlayerController Create(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        PlayerController newPlayer = Object.Instantiate(
            playerPrefab,
            position, Quaternion.identity,
            playerContainer
        );
        
        diContainer.InjectGameObject(newPlayer.gameObject);
        
        newPlayer.Initialize(mainCameraJumper, timerController, playerContainer);
        
        PlaceManager.Instance.AttachToSheet(newPlayer.gameObject, sheet, false);
        newPlayer.Sheet = sheet;
        
        return newPlayer;
    }
}