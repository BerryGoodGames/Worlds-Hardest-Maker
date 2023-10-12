using System;
using System.Collections.Generic;
using System.IO;
using MyBox;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelCardController : MonoBehaviour, IPointerClickHandler
{
    [Separator("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text creatorText;
    [SerializeField] private TMP_Text lastEditedText;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    public string Description
    {
        get => descriptionText.text;
        set => descriptionText.text = value;
    }

    public string Creator
    {
        get => creatorText.text;
        set => creatorText.text = value;
    }

    public string LastEdited
    {
        get => lastEditedText.text;
        set => lastEditedText.text = value;
    }

    [Space] [SerializeField] private LevelCardTween levelCardTween;
    [Space] [SerializeField] private List<Tooltip> buttonTooltips;

    [HideInInspector] public string LevelPath;

    public string LevelName => Path.GetFileName(LevelPath);

    public void EditLevel()
    {
        levelCardTween.OnStartEditing();

        TransitionManager.Instance.LoadLevelPath = LevelPath;
        MainMenuManager.Instance.StartEditor();
    }

    public void OpenDeleteWarning()
    {
        LevelHubManager.Instance.CurrentDeletingLevelCard = this;
        LevelHubManager.Instance.DeleteWarningPrompt.Tween.SetVisible(true);
        LevelHubManager.Instance.DeleteWarningBlockerTween.SetVisible(true);

        LevelHubManager.Instance.DeleteWarningPrompt.ConfirmationText.text =
            $"Do you really want to delete \"{Name}\"?";
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

        while (File.Exists(SaveSystem.LevelSavePath + newName + ".lvl"))
        {
            newName = newName.GetCopyName();
        }

        // copy the data
        if (LevelPath != null)
        {
            string copyPath = SaveSystem.LevelSavePath + newName + ".lvl";

            File.Copy(LevelPath, copyPath, true);
        }

        LevelListLoader.Instance.Refresh();
    }

    public void ExportLevel()
    {
        string exportPath = StandaloneFileBrowser.SaveFilePanel("Export Level",
            Environment.SpecialFolder.UserProfile + "\\Downloads", LevelName, "lvl");


        if (exportPath != string.Empty)
            File.Copy(LevelPath, exportPath, true);
    }

    

    private void Awake()
    {
        foreach (Tooltip tooltip in buttonTooltips)
        {
            tooltip.Container = LevelHubManager.Instance.TooltipContainer;
        }
    }

    public void OnPointerClick(PointerEventData eventData) => levelCardTween.ToggleExpandCollapse();
}