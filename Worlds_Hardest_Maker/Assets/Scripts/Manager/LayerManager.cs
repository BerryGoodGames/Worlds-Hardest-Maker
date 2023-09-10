using System;
using MyBox;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance { get; private set; }

    public LayerVariables Layers;
    public SortingLayerVariables SortingLayers;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
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
    [InitializationField] [MustBeAssigned] public string Field;
    [InitializationField] [MustBeAssigned] public string Coin;
    [InitializationField] [MustBeAssigned] public string Key;
    [InitializationField] [MustBeAssigned] public string Player;
    [InitializationField] [MustBeAssigned] public string Default;
    [InitializationField] [MustBeAssigned] public string Outline;
    [InitializationField] [MustBeAssigned] public string Anchor;
    [InitializationField] [MustBeAssigned] public string Ball;
    [InitializationField] [MustBeAssigned] public string FillPreview;
    [InitializationField] [MustBeAssigned] public string PlacementPreview;
    [InitializationField] [MustBeAssigned] public string Line;
}