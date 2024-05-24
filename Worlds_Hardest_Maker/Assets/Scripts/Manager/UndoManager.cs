using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    private Stack<List<Data>> gameDataStack;
    
    private void Start()
    {
        LevelSessionEditManager.Instance.OnEditAction += PushCurrentGameData;
        LevelSessionManager.Instance.OnLevelLoaded += PushCurrentGameData;
    }
    
    private void Undo()
    {
        if (gameDataStack.Count < 2)
        {
            print("Nothing left to undo");
            return;
        }
        
        // remove current game data to get previous
        gameDataStack.Pop();
        
        List<Data> targetData = gameDataStack.Peek();
        GameManager.Instance.LoadLevelFromDataRaw(targetData);
    }
    
    private void PushCurrentGameData()
    {
        List<Data> newData = SaveSystem.SerializeCurrentLevel();
        
        List<Data> currentData = gameDataStack.Count > 0 ? gameDataStack.Peek() : null;
        
        if (gameDataStack.Count > 0 && CompareData(currentData, newData)) return;
        
        gameDataStack.Push(newData);
    }
    
    private static bool CompareData(List<Data> list1, List<Data> list2)
    {
        if (list1.Count != list2.Count) return false;
        
        for (int i = 0; i < list1.Count; i++)
        {
            Data l1 = list1[i];
            Data l2 = list2[i];
            if (!(l1.GetType().IsInstanceOfType(l2) || l2.GetType().IsInstanceOfType(l1)) || !l1.Equals(l2)) return false;
        }
        
        return true;
    }
    
    private void Update()
    {
        if (KeyBinds.GetKeyBindDown("Editor_Undo")) Undo();
    }
    
    private void Awake() => gameDataStack = new();
}