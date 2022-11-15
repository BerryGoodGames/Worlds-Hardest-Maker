using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages Discord stuff
/// </summary>
public class DiscordManager : MonoBehaviour
{
    // conntect to application (i think)
    public Discord.Discord discord = new Discord.Discord(1027577124812496937, (ulong)Discord.CreateFlags.Default);
    Discord.ActivityManager activityManager;

    public static DiscordManager Instance { get; private set; }

    private static string details = "";
    /// <summary>
    /// what the player is currently doing
    /// </summary>
    public static string Details
    {
        get => details;

        set
        {
            SetActivity(value, currentActivity.State);
        }
    }
    private static string state = "";
    /// <summary>
    /// the player's current status
    /// </summary>
    public static string State 
    { 
        get => state;

        set 
        {
            SetActivity(currentActivity.Details, value);  
        } 
    }

    public static Discord.Activity currentActivity;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else DestroyImmediate(this);

        activityManager = discord.GetActivityManager();

        ClearActivity();

        State = "Making level";
#if UNITY_EDITOR
        Details = "Developing game!";
#endif
    }

    /// <summary>
    /// Sets the activity in discord
    /// </summary>
    /// <param name="details">what the player is currently doing</param>
    /// <param name="state">the player's current status</param>
    public static void SetActivity(string details = "", string state = "")
    {
        currentActivity = new Discord.Activity { Details = details, State = state };
        Instance.activityManager.UpdateActivity(currentActivity, (res) =>
        {
            if (res != Discord.Result.Ok)
            {
                Debug.LogError("Discord status failed!");
            }
        });
    }
    /// <summary>
    /// Clears activity
    /// </summary>
    public static void ClearActivity()
    {
        Instance.activityManager.ClearActivity((res) =>
        {
            if (res != Discord.Result.Ok)
            {
                Debug.LogError("Failed to clear activity!");
            }
            else
            {
                currentActivity = new Discord.Activity { };
            }
        });
    }
    private void Update()
    {
        discord.RunCallbacks();
    }
}
