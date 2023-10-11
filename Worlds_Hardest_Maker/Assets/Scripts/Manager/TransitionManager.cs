using MyBox;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    public static bool HasLoaded => Instance != null;

    [ReadOnly] public bool HasMainMenuStartSwipe;
    [ReadOnly] public string LoadLevelPath = string.Empty;
    [ReadOnly] public bool HasCreatedNewLevel;

    private void Awake()
    {
        if (Instance != null) return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
