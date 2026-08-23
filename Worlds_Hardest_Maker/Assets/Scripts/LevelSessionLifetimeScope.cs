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

        builder.RegisterComponentInHierarchy<CoinManager>()
            .As<ILevelObjectManager>()
            .As<ILevelObjectSerializer>();
        builder.RegisterComponentInHierarchy<BallManager>()
            .As<ILevelObjectManager>()
            .As<ILevelObjectSerializer>();
        builder.RegisterComponentInHierarchy<AnchorManager>()
            .As<ILevelObjectManager>()
            .As<ILevelObjectSerializer>();
        builder.RegisterComponentInHierarchy<PlayerManager>()
            .As<ILevelObjectManager>()
            .As<ILevelObjectSerializer>();
        builder.RegisterComponentInHierarchy<KeyManager>()
            .As<ILevelObjectManager>()
            .As<ILevelObjectSerializer>();
        builder.RegisterComponentInHierarchy<FieldManager>()
            .As<ILevelObjectManager>()
            .As<ILevelObjectSerializer>();
        builder.Register<DeleteFieldManager>(Lifetime.Singleton).As<ILevelObjectManager>();

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