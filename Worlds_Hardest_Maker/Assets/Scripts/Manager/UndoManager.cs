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
        gameDataStack.Push(SaveSystem.SerializeCurrentLevel());
    }
    
    private void Update()
    {
        if (KeyBinds.GetKeyBindDown("Editor_Undo")) Undo();
    }

    private void Awake()
    {
        gameDataStack = new();
    }
}
