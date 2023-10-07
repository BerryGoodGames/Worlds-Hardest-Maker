using System;
using System.IO;
using MyBox;
using SFB;
using UnityEngine;

public class LevelHubManager : MonoBehaviour
{
    public static LevelHubManager Instance { get; private set; }

    public static string LoadedLevelPath = string.Empty;

    [InitializationField][MustBeAssigned] public WarningConfirmPromptController DeleteWarningPrompt;
    [InitializationField][MustBeAssigned] public AlphaTween DeleteWarningBlockerTween;
    [HideInInspector] public LevelCardController CurrentDeletingLevelCard;

    public void ImportLevel()
    {
        // ask user for path
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import Level",
            Environment.SpecialFolder.UserProfile + "\\Downloads", "lvl", false);

        // import selected level
        if (paths.Length <= 0) return;
        ImportLevel(paths[0]);
    }
    public static void ImportLevel(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Path for importing does not exist");
            return;
        }
        
        string fileName = Path.GetFileName(filePath)[..^4];

        // generate copy name if needed
        while (File.Exists(SaveSystem.LevelSavePath + fileName + ".lvl"))
        {
            fileName = fileName.GetCopyName();
        }
        
        // copy file to level save path
        string newFilePath = SaveSystem.LevelSavePath + fileName + ".lvl";
        File.Copy(filePath,  newFilePath);

        // update level list
        LevelListLoader.Instance.Refresh();
    }

    public void CancelDeletion()
    {
        DeleteWarningPrompt.Tween.SetVisible(false);
        DeleteWarningBlockerTween.SetVisible(false);

        CurrentDeletingLevelCard = null;
    }

    public void DeleteCard()
    {
        DeleteWarningPrompt.Tween.SetVisible(false);
        DeleteWarningBlockerTween.SetVisible(false);

        if (CurrentDeletingLevelCard != null) CurrentDeletingLevelCard.DeleteLevel();
        CurrentDeletingLevelCard = null;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
