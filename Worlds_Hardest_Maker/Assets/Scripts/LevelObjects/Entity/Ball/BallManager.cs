using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BallManager : MonoBehaviour, 
    IManager<BallController>, 
    ILevelObjectManager,
    ILevelObjectSerializer
{
    public static BallManager Instance { get; private set; }

    // TODO: why is this a GameObject and not a BallController?
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject ballPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform ballContainer;
    
    [ReadOnly] public List<BallController> BallList;
    [ReadOnly] public Dictionary<AnchorController, List<BallController>> BallListSheets;
    [ReadOnly] public List<BallController> BallListGlobal;
    
    private IObjectResolver diContainer;
    
    [Inject]
    private void Construct(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    #region Set, Get
    
    public BallController SetInSheet(ManagerParameters args)
    {
        if (GetInSheet(args.Position, args.Sheet) != null) return null;
        
        BallController ballController = InstantiateInSheet(args);
        
        // setup parent
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            ballController.ParentAnchor = args.Sheet;
            args.Sheet.Balls.Add(ballController.LevelObject.transform);
        }
        
        ballController.transform.position = args.Position;
        
        // track ball positions in all the layers
        BallList.Add(ballController);
        
        if (AnchorAttachManager.Instance.InAttachMode) BallListSheets[AnchorManager.Instance.SelectedAnchor].Add(ballController);
        else BallListGlobal.Add(ballController);
        
        PlaceManager.Instance.AttachToSheet(ballController.LevelObject, args.Sheet);
        
        return ballController;
    }
    
    public BallController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("BallObject")) continue;
            if (!hit.TryGetComponent(out BallController ball)) continue;
            if (IManager.IsInSheet(ball.transform.parent, sheet)) return ball;
        }
        
        return null;
    }
    
    public BallController InstantiateInSheet(ManagerParameters args)
    {
        Transform container = args.Sheet == null ? ballContainer : args.Sheet.AttachmentContainer;
        
        GameObject ball = Instantiate(
            ballPrefab,
            args.Position, Quaternion.identity,
            container
        );
        
        diContainer.InjectGameObject(ball);
        
        return ball.GetComponentInChildren<BallController>();
    }
    
    public LevelObjectController PlaceLevelObject(ManagerParameters args) => SetInSheet(args);
    
    public List<Data> Serialize(List<Data> levelData)
    {
        if (BallListGlobal == null) return levelData;
        foreach (BallController ball in BallListGlobal)
        {
            if (ball.IsAttached) continue;
            
            BallData ballData = (BallData)ball.GetData();
            levelData.Add(ballData);
        }
        
        return levelData;
    }
    
    #endregion
    
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
    
    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Ball;
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Ball;
    }

    public bool Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition
        });
        
        return SetInSheet(args);
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Data> Serialize()
    {
        throw new System.NotImplementedException();
    }
}