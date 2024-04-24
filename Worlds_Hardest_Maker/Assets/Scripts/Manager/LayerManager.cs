using System;
using System.Reflection;
using JetBrains.Annotations;
using MyBox;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance { get; private set; }

    public LayerVariables Layers;
    public SortingLayerVariables SortingLayers;

    [field: SerializeField] [field: ReadOnly] public string[] AllSortingLayerNames { get; private set; }
    [field: SerializeField] [field: ReadOnly] public int[] AllSortingLayerIDs { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    #if UNITY_EDITOR
    
    [ButtonMethod] [UsedImplicitly]
    public void UpdateSortingLayerLists()
    {
        AllSortingLayerNames = GetSortingLayerNames();
        AllSortingLayerIDs = GetSortingLayerUniqueIDs();
        print("Successfully updated sorting layer lists");
    }
    
    private static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, Array.Empty<object>());
    }

    private static int[] GetSortingLayerUniqueIDs()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty(
            "sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic
        );

        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    }
    
    #endif
}

[Serializable]
public class LayerVariables
{
    [InitializationField] [MustBeAssigned] public LayerMask Default;
    [InitializationField] [MustBeAssigned] public LayerMask TransparentFX;
    [InitializationField] [MustBeAssigned] public LayerMask IgnoreRaycast;
    [InitializationField] [MustBeAssigned] public LayerMask Water;
    [InitializationField] [MustBeAssigned] public LayerMask UI;
    [InitializationField] [MustBeAssigned] public LayerMask Entity;
    [InitializationField] [MustBeAssigned] public LayerMask Player;
    [InitializationField] [MustBeAssigned] public LayerMask Void;
    [InitializationField] [MustBeAssigned] public LayerMask Field;
    [InitializationField] [MustBeAssigned] public LayerMask Background;
    [InitializationField] [MustBeAssigned] public LayerMask DoNotCollide;
}

[Serializable]
public class SortingLayerVariables
{
    [InitializationField] [MustBeAssigned] public string Background;
    [InitializationField] [MustBeAssigned] public string AnchorBelow;
    [InitializationField] [MustBeAssigned] public string Field;
    [InitializationField] [MustBeAssigned] public string Coin;
    [InitializationField] [MustBeAssigned] public string Key;
    [InitializationField] [MustBeAssigned] public string Player;
    [InitializationField] [MustBeAssigned] public string Default;
    [InitializationField] [MustBeAssigned] public string Outline;
    [InitializationField] [MustBeAssigned] public string Anchor;
    [InitializationField] [MustBeAssigned] public string Ball;
    [InitializationField] [MustBeAssigned] public string AnchorAbove;
    [InitializationField] [MustBeAssigned] public string PlayerPlayMode;
    [InitializationField] [MustBeAssigned] public string FillPreview;
    [InitializationField] [MustBeAssigned] public string PlacementPreview;
    [InitializationField] [MustBeAssigned] public string Line;
}