using System;
using System.IO;
using System.Runtime.InteropServices;
using MyBox;
using SFB;
using TMPro;
using UnityEngine;

public class LevelCardController : MonoBehaviour
{
    [Separator("References")]
    [SerializeField] private TMP_Text nameText;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    [HideInInspector] public string LevelPath;

    public string LevelName => Path.GetFileName(LevelPath);

    public void EditLevel()
    {
        LevelHubManager.LoadedLevelPath = LevelPath;
        MainMenuManager.Instance.StartEditor();
    }

    public void OpenDeleteWarning()
    {
        LevelHubManager.Instance.CurrentDeletingLevelCard = this;
        LevelHubManager.Instance.DeleteWarningPrompt.Tween.SetVisible(true);
        LevelHubManager.Instance.DeleteWarningBlockerTween.SetVisible(true);

        LevelHubManager.Instance.DeleteWarningPrompt.ConfirmationText.text = $"Do you really want to delete \"{Name}\"?";
    }

    public void DeleteLevel()
    {
        File.Delete(LevelPath);
        LevelListLoader.Instance.Refresh();
    }

    public void CopyLevel()
    {
        // get new name
        string newName = LevelName[..^4].GetCopyName();

        while (File.Exists(newName + ".lvl"))
        {
            newName = newName.GetCopyName();
        }

        // copy the data
        if (LevelPath != null) 
            File.Copy(LevelPath, SaveSystem.LevelSavePath + newName + ".lvl", true);

        LevelListLoader.Instance.Refresh();
    }

    public void ExportLevel()
    {
        string exportPath = StandaloneFileBrowser.SaveFilePanel("Export Level",
            Environment.SpecialFolder.UserProfile + "\\Downloads", LevelName, "lvl");
        

        if(exportPath != string.Empty)
            File.Copy(LevelPath, exportPath, true);
    }
}
