using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerFactory
{
    private readonly IObjectResolver diContainer;
    private readonly IAttachmentService attachmentService;
    
    private PlayerController playerPrefab;
    private JumpToEntity mainCameraJumper;
    private TimerController timerController;
    private Transform playerContainer;

    public PlayerFactory(IObjectResolver diContainer, IAttachmentService attachmentService)
    {
        this.diContainer = diContainer;
        this.attachmentService = attachmentService;
    }
    
    public void Initialize(PlayerController playerPrefab, JumpToEntity mainCameraJumper, TimerController timerController, Transform playerContainer)
    {
        this.playerPrefab = playerPrefab;
        this.mainCameraJumper = mainCameraJumper;
        this.timerController = timerController;
        this.playerContainer = playerContainer;
    }
    
    public PlayerController Create(Vector2 position, ISheet sheet)
    {
        PlayerController newPlayer = Object.Instantiate(
            playerPrefab,
            position, Quaternion.identity,
            playerContainer
        );
        
        diContainer.InjectGameObject(newPlayer.gameObject);
        
        newPlayer.Initialize(mainCameraJumper, timerController, playerContainer);
        
        if (sheet is AnchorSheet anchorSheet) attachmentService.Attach(newPlayer, anchorSheet.Anchor, false);
        newPlayer.Sheet = sheet;
        
        return newPlayer;
    }
}