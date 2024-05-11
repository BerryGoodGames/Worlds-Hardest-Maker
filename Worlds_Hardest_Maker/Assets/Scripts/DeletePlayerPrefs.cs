using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour
{
    [Button]
    [UsedImplicitly]
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        print("All player prefs have been deleted!");
    }
}