using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyBox;
using SFB;
using UnityEngine;

public class LevelHubManager : MonoBehaviour
{
    public static string LoadedLevelPath = string.Empty;

    public void ImportLevel()
    {
        // ask user for path
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import Level",
            Environment.SpecialFolder.UserProfile + @"\Downloads", "lvl", false);

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
            fileName = GetCopyName(fileName);
        }
        
        // copy file to level save path
        string newFilePath = SaveSystem.LevelSavePath + fileName + ".lvl";
        File.Copy(filePath,  newFilePath);

        // update level list
        LevelListLoader.Instance.Refresh();
    }

    private static string GetCopyName(string fileName)
    {
        int copyNumber = 0;
        const string pattern = " (";

        int indexOfPattern = fileName.LastIndexOf(pattern, StringComparison.Ordinal);

        if (indexOfPattern != -1)
        {
            int endIndex = fileName.LastIndexOf(')');
            if (endIndex != -1 && endIndex > indexOfPattern)
            {
                string numberString = fileName.Substring(indexOfPattern + pattern.Length, endIndex - indexOfPattern - pattern.Length);
                if (int.TryParse(numberString, out copyNumber))
                {
                    fileName = fileName.Remove(indexOfPattern);
                }
            }
        }

        fileName += $" ({copyNumber + 1})";

        return fileName;
    }
}
