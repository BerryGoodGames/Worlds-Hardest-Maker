using System.IO;
using MyBox;
using TMPro;
using UnityEngine;

public class LevelCreationController : MonoBehaviour
{
    private static readonly string defaultLevelPath = Application.dataPath + "/Resources/DefaultLevel.lvl";

    [Separator("References")] [SerializeField] private TMP_InputField levelNameText;

    [SerializeField] private TMP_InputField descriptionText;
    [SerializeField] private TMP_InputField creatorText;

    public void CreateLevel()
    {
        string levelName = levelNameText.text;

        CreateLevel(ref levelName);

        TransitionManager.Instance.LoadLevelPath = SaveSystem.LevelSavePath + levelName + ".lvl";
        TransitionManager.Instance.HasCreatedNewLevel = true;
        TransitionManager.Instance.Inputs = new()
        {
            Name = levelName,
            Description = descriptionText.text,
            Creator = creatorText.text,
        };

        MainMenuManager.Instance.OpenLevelScene();
    }

    private static void CreateLevel(ref string name)
    {
        // make sure that the file doesn't already exist
        while (File.Exists(SaveSystem.LevelSavePath + name + ".lvl")) { name = name.GetCopyName(); }

        string levelPath = SaveSystem.LevelSavePath + name + ".lvl";

        File.Copy(defaultLevelPath, levelPath);
    }
}