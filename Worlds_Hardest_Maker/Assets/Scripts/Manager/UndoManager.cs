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
            if (!l1.Equals(l2)) return false;
        }

        return true;
    }

    // private static bool CompareData(List<Data> list1, List<Data> list2)
    // {
    //     List<Data> firstNotSecond = list1.Except(list2).ToList();
    //     List<Data> secondNotFirst = list2.Except(list1).ToList();
    //     return !firstNotSecond.Any() && !secondNotFirst.Any();
    // }

    // private static bool CompareData(List<Data> list1, List<Data> list2)
    // {
    //     return list1.SequenceEqual(list2);
    // }
    
    // private static bool CompareData(List<Data> list1, List<Data> list2)
    // {
    //     IEnumerable<Data> inListButNotInList2 = list1.Except(list2);
    //     IEnumerable<Data> inList2ButNotInList = list2.Except(list1);
    //
    //     IEnumerable<Data> first10 = inListButNotInList2.Take(10);
    // }
    
    private void Update()
    {
        if (KeyBinds.GetKeyBindDown("Editor_Undo")) Undo();
    }

    private void Awake()
    {
        gameDataStack = new();
    }
}
