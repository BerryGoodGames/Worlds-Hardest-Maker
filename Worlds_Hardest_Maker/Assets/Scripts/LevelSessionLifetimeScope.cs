using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using WorldsHardestMaker.CopyPaste;
using WorldsHardestMaker.Panels;
using WorldsHardestMaker.PlayerRecording;
using WorldsHardestMaker.Selection;

public class LevelSessionLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<KonamiManager>().As<IKonamiService>();
        
        // register preview services
        builder.Register<PreviewSpriteDataProvider>(Lifetime.Singleton);
        builder.Register<PreviewRotationDataProvider>(Lifetime.Singleton);
        builder.Register<PreviewVisibilityRulesService>(Lifetime.Singleton);
        builder.Register<PreviewSpriteAlphaProvider>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<MouseManager>().As<IMouseService>();

        builder.RegisterComponentInHierarchy<PlayerRecordingManager>().As<IRecordingService>();

        builder.Register<SelectionState>(Lifetime.Singleton).As<ISelectionStateService, ISelectionAreaProvider>();

        builder.Register<PositionQueryService>(Lifetime.Singleton).As<IPositionQueryService>();
        builder.Register<AreaQueryService>(Lifetime.Singleton).As<IAreaQueryService>();
        
        builder.Register<AreaErasureService>(Lifetime.Singleton).As<IAreaErasureService>();
        builder.Register<AreaFillService>(Lifetime.Singleton).As<IAreaFillService>();

        builder.RegisterComponentInHierarchy<DrawManager>().As<IDrawService>();

        builder.RegisterComponentInHierarchy<CopyPasteManager>().As<ICopyPasteService>();

        builder.RegisterComponentInHierarchy<EditModeUIBlockerService>().As<IEditModeUIBlockerService>();

        builder.Register<CopyDataFactory>(Lifetime.Singleton).AsSelf();

        builder.RegisterComponentInHierarchy<PanelManager>().As<IPanelService>();
        builder.Register<PanelRegistry>(Lifetime.Singleton).As<IPanelRegistry>();

        builder.Register<AnchorBlockViewFactory>(Lifetime.Singleton).AsSelf();
        builder.Register<AnchorChainDropTargetResolver>(Lifetime.Singleton).AsSelf();

        // level object registering
        builder.RegisterComponentInHierarchy<CoinManager>().AsImplementedInterfaces();
        builder.Register<CoinQueryService>(Lifetime.Singleton).As<ILevelObjectQuery<CoinController>>().AsSelf();
        builder.Register<CoinFactory>(Lifetime.Singleton).As<ILevelObjectFactory<CoinController>>().AsSelf();
        builder.Register<CoinPlacementRules>(Lifetime.Singleton).AsSelf();
        
        builder.RegisterComponentInHierarchy<BallManager>().AsImplementedInterfaces();
        builder.Register<BallQueryService>(Lifetime.Singleton).As<ILevelObjectQuery<BallController>>();
        builder.Register<BallFactory>(Lifetime.Singleton).As<ILevelObjectFactory<BallController>>().AsSelf();
        
        builder.RegisterComponentInHierarchy<AnchorManager>().AsImplementedInterfaces();
        builder.Register<AnchorQueryService>(Lifetime.Singleton).As<ILevelObjectQuery<AnchorController>>();
        builder.Register<AnchorFactory>(Lifetime.Singleton).As<ILevelObjectFactory<AnchorController>>().AsSelf();
        
        builder.RegisterComponentInHierarchy<PlayerManager>().AsImplementedInterfaces();
        builder.Register<PlayerQueryService>(Lifetime.Singleton).As<ILevelObjectQuery<PlayerController>>();
        builder.Register<PlayerFactory>(Lifetime.Singleton).As<ILevelObjectFactory<PlayerController>>().AsSelf();
        builder.Register<PlayerPlacementRules>(Lifetime.Singleton).AsSelf();
        
        builder.RegisterComponentInHierarchy<KeyManager>().AsImplementedInterfaces();
        builder.Register<KeyQueryService>(Lifetime.Singleton).As<ILevelObjectQuery<KeyController>>().AsSelf();
        builder.Register<KeyFactory>(Lifetime.Singleton).As<ILevelObjectFactory<KeyController>>().AsSelf();
        builder.Register<KeyPlacementRules>(Lifetime.Singleton).AsSelf();
        
        builder.RegisterComponentInHierarchy<FieldManager>().AsImplementedInterfaces();
        builder.Register<FieldQueryService>(Lifetime.Singleton).As<ILevelObjectQuery<FieldController>>().AsSelf();
        builder.Register<FieldFactory>(Lifetime.Singleton).As<ILevelObjectFactory<FieldController>>().AsSelf();
        builder.Register<DeleteFieldManager>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.Register<SaveSystem>(Lifetime.Singleton);
    }
    
    protected override void Awake()
    {
        base.Awake();
        
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject o in roots)
        {
            Container.InjectGameObject(o);
        }
    }
}