using Discord;
using UnityEngine;

/// <summary>
///     Manages Discord stuff
/// </summary>
public class DiscordManager : MonoBehaviour
{
    // connect to application (i think)
    public Discord.Discord Discord = new(1027577124812496937, (ulong)CreateFlags.Default);
    private ActivityManager activityManager;

    public static DiscordManager Instance { get; private set; }

    private static string details = "";

    /// <summary>
    ///     what the player is currently doing
    /// </summary>
    public static string Details
    {
        get => details;
        set => SetActivity(value, CurrentActivity.State);
    }

    private static string state = "";

    /// <summary>
    ///     the player's current status
    /// </summary>
    public static string State
    {
        get => state;
        set => SetActivity(CurrentActivity.Details, value);
    }

    public static Activity CurrentActivity;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else DestroyImmediate(this);

        activityManager = Discord.GetActivityManager();

        ClearActivity();

        State = "Making level";
#if UNITY_EDITOR
        State = "";
        Details = "Developing!";
#endif
    }

    /// <summary>
    ///     Sets the activity in discord
    /// </summary>
    /// <param name="details">what the player is currently doing</param>
    /// <param name="state">the player's current status</param>
    public static void SetActivity(string details = "", string state = "")
    {
        CurrentActivity = new Activity { Details = details, State = state };
        Instance.activityManager.UpdateActivity(CurrentActivity, res =>
        {
            if (res != Result.Ok)
            {
                Debug.LogError("Discord status failed!");
            }
        });
    }

    /// <summary>
    ///     Clears activity
    /// </summary>
    public static void ClearActivity()
    {
        Instance.activityManager.ClearActivity(res =>
        {
            if (res != Result.Ok)
            {
                Debug.LogError("Failed to clear activity!");
            }
            else
            {
                CurrentActivity = new Activity();
            }
        });
    }

    private void Update()
    {
        Discord.RunCallbacks();
    }
}