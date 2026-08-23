using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class UndoManager : MonoBehaviour
{
    private Stack<List<Data>> gameDataUndoStack;
    private Stack<List<Data>> gameDataRedoStack;
    
    private EventBus eventBus;
    private SaveSystem saveSystem;
    
    [Inject]
    private void Construct(EventBus eventBus, SaveSystem saveSystem)
    {
        this.eventBus = eventBus;
        this.saveSystem = saveSystem;
        
        eventBus.Subscribe<EditActionEvent>(OnEditAction);
    }
    
    private void Start()
    {
        LevelSessionManager.Instance.OnLevelLoaded += PushCurrentGameData;
    }
    
    private void OnEditAction(EditActionEvent evt) => PushCurrentGameData();
    
    private void Undo()
    {
        if (gameDataUndoStack.Count <= 1) return;
        
        // remove current game data to get previous
        gameDataRedoStack.Push(gameDataUndoStack.Pop());
        
        List<Data> targetData = gameDataUndoStack.Peek();
        GameManager.Instance.LoadLevelFromDataRaw(targetData);
        
    }

    private void Redo()
    {
        if (gameDataRedoStack.Count <= 0) return;
        
        List<Data> targetData = gameDataRedoStack.Pop();
        GameManager.Instance.LoadLevelFromDataRaw(targetData);
        gameDataUndoStack.Push(targetData);
    }
    
    private void PushCurrentGameData()
    {
        List<Data> newData = saveSystem.SerializeCurrentLevel();
        
        List<Data> currentData = gameDataUndoStack.Count > 0 ? gameDataUndoStack.Peek() : null;
        
        if (gameDataUndoStack.Count > 0 && CompareData(currentData, newData)) return;
        
        gameDataUndoStack.Push(newData);
        gameDataRedoStack.Clear();
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
        if (KeyBinds.GetKeyBindDown("Editor_Redo")) Redo();
    }
    
    private void Awake()
    {
        gameDataUndoStack = new();
        gameDataRedoStack = new();
    }

    private void OnDestroy()
    {
        eventBus.Unsubscribe<EditActionEvent>(OnEditAction);
        
        LevelSessionManager.Instance.OnLevelLoaded -= PushCurrentGameData;
    }
}