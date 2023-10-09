using System;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    public static bool HasLoaded => Instance != null;

    [ReadOnly] public bool HasMainMenuStartSwipe;
    [FormerlySerializedAs("LevelLoadString")] [ReadOnly] public string LoadLevelPath = string.Empty;

    private void Awake()
    {
        if (Instance != null) return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
