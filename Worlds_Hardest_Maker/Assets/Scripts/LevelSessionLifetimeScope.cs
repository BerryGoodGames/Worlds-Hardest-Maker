using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
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