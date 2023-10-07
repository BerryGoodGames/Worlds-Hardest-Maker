using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelCreationController : MonoBehaviour
{
    private static readonly string defaultLevelPath = Application.dataPath + "/Resources/DefaultLevel.lvl";

    [FormerlySerializedAs("levelName")] [FormerlySerializedAs("LevelName")] [Separator("References")] [SerializeField]
    private TMP_InputField levelNameText;

    public void CreateLevel()
    {
        string levelName = levelNameText.text;

        CreateLevel(levelName);

        LevelHubManager.LoadedLevelPath = SaveSystem.LevelSavePath + levelName + ".lvl";
        MainMenuManager.Instance.StartEditor();
    }

    private static void CreateLevel(string name)
    {
        // make sure that the file doesn't already exist
        while (File.Exists(SaveSystem.LevelSavePath + name + ".lvl"))
        {
            name = name.GetCopyName();
        }

        string levelPath = SaveSystem.LevelSavePath + name + ".lvl";

        File.Copy(defaultLevelPath, levelPath);
    }

}
