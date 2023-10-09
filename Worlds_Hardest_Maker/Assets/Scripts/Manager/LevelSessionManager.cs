using MyBox;
using UnityEngine;

/// <summary>
/// This manager exists to assure that the session data (e.g. path of currently edited level) is always available and loaded,
/// unlike TransitionManager which is only loaded when the scene is entered after main menu
/// </summary>
public class LevelSessionManager : MonoBehaviour
{
    public static LevelSessionManager Instance { get; private set; }

    public static bool IsSessionFromEditor => !TransitionManager.HasLoaded;

    [ReadOnly] public string LevelSessionPath = string.Empty;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
