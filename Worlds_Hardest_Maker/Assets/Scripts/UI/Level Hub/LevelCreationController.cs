using System.IO;
using MyBox;
using TMPro;
using UnityEngine;

public class LevelCreationController : MonoBehaviour
{
    // private static readonly string defaultLevelPath = Application.dataPath + "/Resources/DefaultLevel.lvl";

    [Separator("References")] [SerializeField] private TMP_InputField levelNameText;
    [SerializeField] private TMP_InputField descriptionText;
    [SerializeField] private TMP_InputField creatorText;

    public void CreateLevel()
    {
        string levelName = MakeNameUnique(levelNameText.text.Trim());
        
        CreateLevel(levelName, descriptionText.text.Trim(), creatorText.text.Trim());

        TransitionManager.Instance.LoadLevelPath = SaveSystem.LevelSavePath + levelName + ".lvl";
        TransitionManager.Instance.LevelSessionMode = LevelSessionMode.Edit;

        MainMenuManager.Instance.OpenLevelScene();
    }

    private static void CreateLevel(string name, string description, string creator)
    {
        string levelPath = SaveSystem.LevelSavePath + name + ".lvl";

        LevelData levelData = new()
        {
            Info = new()
            {
                Description = description,
                Creator = creator,
            },
            Objects = new(),
        };

        SaveSystem.SerializeLevelData(levelPath, levelData);
    }

    private static string MakeNameUnique(string name)
    {
        // make sure that the file doesn't already exist
        while (File.Exists(SaveSystem.LevelSavePath + name + ".lvl")) name = name.GetCopyName();

        return name;
    }
}