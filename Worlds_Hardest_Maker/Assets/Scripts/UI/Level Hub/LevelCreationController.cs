using System.IO;
using MyBox;
using TMPro;
using UnityEngine;

public class LevelCreationController : MonoBehaviour
{
    // private static readonly string defaultLevelPath = Application.dataPath + "/Resources/DefaultLevel.lvl";
    
    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField levelNameText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField descriptionText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_InputField creatorText;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput roomWidthNumberInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput roomHeightNumberInput;
    
    
    public void CreateLevel()
    {
        string levelName = MakeNameUnique(levelNameText.text.Trim());
        
        CreateLevel(levelName, descriptionText.text.Trim(), creatorText.text.Trim());
        
        TransitionManager.Instance.LoadLevelPath = SaveSystem.LevelSavePath + levelName + ".lvl";
        TransitionManager.Instance.LevelSessionMode = LevelSessionMode.Edit;
        TransitionManager.Instance.RoomSize = new((int)roomWidthNumberInput.GetCurrentNumber(), (int)roomHeightNumberInput.GetCurrentNumber());
        
        MainMenuManager.Instance.OpenLevelScene();
    }
    
    public static void CreateLevel(string name, string description, string creator)
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