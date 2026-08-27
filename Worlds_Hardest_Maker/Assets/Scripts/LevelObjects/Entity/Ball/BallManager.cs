using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static BallManager Instance { get; private set; }

    // TODO: why is this a GameObject and not a BallController?
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject ballPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform ballContainer;
    
    [ReadOnly] public List<BallController> BallList;
    [ReadOnly] public Dictionary<AnchorController, List<BallController>> BallListSheets;
    [ReadOnly] public List<BallController> BallListGlobal;
    
    [Inject] private IObjectResolver diContainer;
    [Inject] private ILevelObjectQuery<BallController> ballQueryService;
    private BallFactory ballFactory;

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
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            ballController.ParentAnchor = sheet.ToAnchorOrNull();
            sheet.ToAnchorOrNull()!.Balls.Add(ballController.LevelObject.transform);
        }
        
        ballController.transform.position = position;
        
        // track ball positions in all the layers
        BallList.Add(ballController);
        
        if (AnchorAttachManager.Instance.InAttachMode) BallListSheets[AnchorManager.Instance.SelectedAnchor].Add(ballController);
        else BallListGlobal.Add(ballController);
        
        PlaceManager.Instance.AttachToSheet(ballController.LevelObject, sheet);
        
        return ballController;
    }
    
    private void Start()
    {
        BallListSheets = new();
        BallListGlobal = new();
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
        // ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        // {
        //     Position = gridPosition
        // });
        //
        // BallController result = SetInSheet(args);
        BallController result = CreateNew(gridPosition, request.Sheet);
        
        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();

        if (BallListGlobal == null) return levelData;
        foreach (BallController ball in BallListGlobal)
        {
            if (ball.IsAttached) continue;
            
            BallData ballData = (BallData)ball.GetData();
            levelData.Add(ballData);
        }
        
        return levelData;
    }
}