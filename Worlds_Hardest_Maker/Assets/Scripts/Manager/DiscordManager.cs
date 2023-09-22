using System;
using Discord;
using MyBox;
using UnityEngine;

[ExecuteAlways]
public class DiscordManager : MonoBehaviour
{
    public static DiscordManager Instance { get; private set; }

    [SerializeField] private long applicationID;
    [Space] [SerializeField] [ReadOnly] private string details;

    public string Details
    {
        get => details;
        set => SetActivity(value, CurrentActivity.State);
    }

    [SerializeField] [ReadOnly] private string state;

    public string State
    {
        get => state;
        set => SetActivity(CurrentActivity.Details, value);
    }

    [Space] [SerializeField] private string largeImage = "dc_logo";
    [SerializeField] private string largeText = "World's Hardest Maker";
    [Space] [SerializeField] private bool printWarnings;

    private long time;

    private static bool instanceExists;
    private Discord.Discord discord;

    private ActivityManager activityManager;
    public Activity CurrentActivity { get; private set; }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else if (Application.isPlaying) DestroyImmediate(this);

        if (!Application.isPlaying) return;

        // Transition the GameObject between scenes, destroy any duplicates
        if (!instanceExists & Application.isPlaying)
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
    }

    private void Start() => Setup();

    private void Update()
    {
        // Destroy the GameObject if Discord isn't running
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            if (Application.isPlaying) Destroy(gameObject);
        }

        if (!Application.isPlaying) UpdateStatus();
    }

    private void LateUpdate()
    {
        if (Application.isPlaying) UpdateStatus();
    }

    private void UpdateStatus()
    {
        // Update Status every frame
        try
        {
#if UNITY_EDITOR
            details = "Developing editor!";
            // state = "Currently " + (Application.isPlaying ? "testing" : "coding") + "!";
            state = "";
#else
            details = "Building level!";
            state = "";
#endif

            Activity activity = new()
            {
                Details = details,
                State = state,
                Assets =
                {
                    LargeImage = largeImage,
                    LargeText = largeText
                },
                Timestamps =
                {
                    Start = time
                }
            };

            activityManager.UpdateActivity(activity, res =>
            {
                if (res != Result.Ok) Debug.LogWarning("Failed connecting to Discord!");
            });
        }
        catch
        {
            // If updating the status fails, Destroy the GameObject (or warning)
            if (Application.isPlaying)
                Destroy(gameObject);
            else if (printWarnings) Debug.LogWarning("Updating status failed!");
        }
    }

    public void ClearActivity() =>
        activityManager.ClearActivity(res =>
        {
            if (res != Result.Ok)
                Debug.LogError("Failed to clear activity!");
            else
                CurrentActivity = new Activity();
        });

    public void SetActivity(string details = "", string state = "")
    {
        CurrentActivity = new Activity { Details = details, State = state };
        activityManager.UpdateActivity(CurrentActivity, res =>
        {
            if (res != Result.Ok) Debug.LogError("Discord status failed!");
        });
    }

    [ButtonMethod]
    public void Setup()
    {
        // Log in with the Application ID
        discord = new Discord.Discord(applicationID, (ulong)CreateFlags.NoRequireDiscord);

        time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        activityManager = discord.GetActivityManager();

        ClearActivity();

        UpdateStatus();
    }

    private void OnDestroy()
    {
        ClearActivity();
    }
}