using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using MyBox;
using TMPro;
using UnityEngine;

public class LevelModifyController : MonoBehaviour
{
    public static LevelModifyController Instance { get; private set; }

    [HideInInspector] public LevelCardController CurrentCard;

    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField levelNameText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField descriptionText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField creatorText;
    [Space] [InitializationField] [MustBeAssigned] public MoveRelativeTween LevelModifyStartTween;
    [InitializationField] [MustBeAssigned] public MoveRelativeTween LevelModifyBackTween;

    public void SaveLevelSettings()
    {
        LevelModifyBackTween.Move();

        string newName = levelNameText.text.Trim();
        string newDescription = descriptionText.text.Trim();
        string newCreator = creatorText.text.Trim();

        string oldPath = CurrentCard.LevelPath;
        string newPath =
            $"{string.Join("\\", CurrentCard.LevelPath.Split("\\").Take(CurrentCard.LevelPath.Split("\\").Length - 1).ToArray())}\\{newName}.lvl";

        File.Move(oldPath, newPath);

        LevelData levelData = SaveSystem.LoadLevel(newPath);
        levelData.Info.Name = newName;
        levelData.Info.Description = newDescription;
        levelData.Info.Creator = newCreator;

        // overwrite file with new description and creator
        SaveSystem.SerializeLevelData(newPath, levelData);

        LevelListLoader.Instance.Refresh(true);
    }

    public void FillInputs(LevelCardController card)
    {
        levelNameText.text = card.Name;
        descriptionText.text = card.Description;
        creatorText.text = card.Creator.Substring(3, card.Creator.Length - 3);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}