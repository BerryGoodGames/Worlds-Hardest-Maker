using System;
using System.IO;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelModifyController : MonoBehaviour
{
    public static LevelModifyController Instance { get; private set; }

    [HideInInspector] public LevelCardController CurrentCard;

    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField levelNameText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField descriptionText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField creatorText;
    [Space] [InitializationField] [MustBeAssigned] public MoveRelativeTween LevelModifyStartTween;
    [InitializationField] [MustBeAssigned] public MoveRelativeTween LevelModifyBackTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle resetPlayStats;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle resetEditorStats;

    public void SaveLevelSettings()
    {
        LevelModifyBackTween.Move();

        string oldPath = CurrentCard.LevelPath;
        string newName = levelNameText.text.Trim();
        string newPath =
            $"{string.Join("\\", CurrentCard.LevelPath.Split("\\").Take(CurrentCard.LevelPath.Split("\\").Length - 1).ToArray())}\\{newName}.lvl";

        string newDescription = descriptionText.text.Trim();
        string newCreator = creatorText.text.Trim();

        if (File.Exists(newPath) && oldPath != newPath)
        {
            newName = newName.GetCopyName();
            newPath =
                $"{string.Join("\\", CurrentCard.LevelPath.Split("\\").Take(CurrentCard.LevelPath.Split("\\").Length - 1).ToArray())}\\{newName}.lvl";
        }

        File.Move(oldPath, newPath);

        LevelData levelData = SaveSystem.LoadLevel(newPath);
        levelData.Info.Description = newDescription;
        levelData.Info.Creator = newCreator;

        if (resetPlayStats.isOn)
        {
            levelData.Info.Completions = 0;
            levelData.Info.Deaths = 0;
            levelData.Info.PlayTime = TimeSpan.Zero;
            levelData.Info.BestCompletionTime = TimeSpan.MaxValue;
        }

        if (resetEditorStats.isOn) levelData.Info.EditTime = TimeSpan.Zero;

        // overwrite file with new description and creator
        SaveSystem.SerializeLevelData(newPath, levelData);

        LevelListLoader.Instance.Refresh(true);
    }

    public void FillInputs(LevelCardController card)
    {
        levelNameText.text = card.Name;
        descriptionText.text = card.Description;
        creatorText.text = card.Creator.Substring(3, card.Creator.Length - 3);

        resetPlayStats.isOn = false;
        resetEditorStats.isOn = false;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}