using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour
{
    [ButtonMethod]
    [UsedImplicitly]
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        print("All player prefs have been deleted!");
    }
}