using System.Collections.Generic;
using System.IO;
using B83.Win32;
using UnityEngine;

public class ExternalFileDragAndDropHandler : MonoBehaviour
{
    private readonly Texture2D[] textures = new Texture2D[6];
    private DropInfo dropInfo;
    
    private class DropInfo
    {
        public List<string> Files;
        public Vector2 Pos;
    }
    
    private void OnEnable ()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;

    }
    
    private void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }
    
    private void OnFiles(List<string> droppedFiles, POINT aPos)
    {
        // List<string> files = new();
        
        // scan through dropped files and filter out supported image types
        foreach(string f in droppedFiles)
        {
            FileInfo fi = new(f);
            string ext = fi.Extension.ToLower();
            if (ext == ".lvl")
            {
                // files.Add(f);
                // break;
                
                LevelHubManager.ImportLevel(f);
            }
        }
        
        // // If the user dropped a supported file, create a DropInfo
        // if (files.Count > 0)
        // {
        //     DropInfo info = new DropInfo
        //     {
        //         Files = files,
        //         Pos = new Vector2(aPos.x, aPos.y),
        //     };
        //     dropInfo = info;
        // }
    }
    
    // private void LoadImage(int aIndex, DropInfo aInfo)
    // {
    //     if (aInfo == null)
    //     {
    //         return;
    //     }
    //     
    //     // get the GUI rect of the last Label / box
    //     Rect rect = GUILayoutUtility.GetLastRect();
    //     
    //     // check if the drop position is inside that rect
    //     if (rect.Contains(aInfo.Pos))
    //     {
    //         byte[] data = File.ReadAllBytes(aInfo.Files);
    //         
    //         Texture2D tex = new Texture2D(1,1);
    //         tex.LoadImage(data);
    //         
    //         if (textures[aIndex] != null)
    //         {
    //             Destroy(textures[aIndex]);
    //         }
    //         
    //         textures[aIndex] = tex;
    //     }
    // }

    // private void OnGUI()
    // {
    //     DropInfo tmp = null;
    //     if (Event.current.type == EventType.Repaint && dropInfo != null)
    //     {
    //         tmp = dropInfo;
    //         dropInfo = null;
    //     }
    //     
    //     GUILayout.BeginHorizontal();
    //     for (int i = 0; i < 3; i++)
    //     {
    //         if (textures[i] != null)
    //         {
    //             GUILayout.Label(textures[i], GUILayout.Width(200), GUILayout.Height(200));
    //         }
    //         else
    //         {
    //             GUILayout.Box("Drag image here", GUILayout.Width(200), GUILayout.Height(200));
    //         }
    //         LoadImage(i, tmp);
    //     }
    //     
    //     GUILayout.EndHorizontal();
    //     GUILayout.BeginHorizontal();
    //     for (int i = 3; i < 6; i++)
    //     {
    //         if (textures[i] != null)
    //         {
    //             GUILayout.Label(textures[i], GUILayout.Width(200), GUILayout.Height(200));
    //         }
    //         else
    //         {
    //             GUILayout.Box("Drag image here", GUILayout.Width(200), GUILayout.Height(200));
    //         }
    //         LoadImage(i, tmp);
    //     }
    //     
    //     GUILayout.EndHorizontal();
    // }
}
