using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour, IBallManager, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static BallManager Instance { get; private set; }

    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject ballPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform ballContainer;

    private readonly SheetScopedRegistry<BallController> ballsBySheet = new();
    public void RemoveFromSheetRegistry(BallController ball) => ballsBySheet.RemoveItem(ball.Sheet, ball);
    
    [Inject] private ILevelObjectQuery<BallController> ballQueryService;
    private BallFactory ballFactory;
    [Inject] private IAttachmentService attachmentService;

    [Inject]
    private void Construct(BallFactory ballFactory)
    {
        this.ballFactory = ballFactory;
        ballFactory.Initialize(ballPrefab, ballContainer);
    }

    public BallController CreateNew(Vector2 position, ISheet sheet)
    {
        if (ballQueryService.Exists(position, sheet)) return null;
        
        BallController ballController = ballFactory.Create(position, sheet);
        
        // setup parent
        if (sheet is AnchorSheet anchorSheet1)
        {
            ballController.ParentAnchor = anchorSheet1.Anchor;
            anchorSheet1.Anchor.Balls.Add(ballController.LevelObject.transform);
        }
        
        ballController.transform.position = position;

        ballsBySheet.AddItem(sheet, ballController);
        
        if (sheet is AnchorSheet anchorSheet2) attachmentService.Attach(ballController, anchorSheet2.Anchor);
        
        return ballController;
    }
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Ball;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        BallController result = CreateNew(gridPosition, request.Sheet);
        
        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();

        foreach (BallController ball in ballsBySheet.Get(GlobalSheet.Instance))
        {
            if (ball.IsAttached) continue;
            
            BallData ballData = (BallData)ball.GetData();
            levelData.Add(ballData);
        }
        
        return levelData;
    }
}