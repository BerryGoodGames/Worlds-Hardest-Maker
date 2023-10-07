using System.IO;
using MyBox;
using TMPro;
using UnityEngine;

public class LevelCardController : MonoBehaviour
{
    [Separator("References")]
    [SerializeField] private TMP_Text nameText;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    [HideInInspector] public string LevelPath;

    public void EditLevel()
    {
        LevelHubManager.LoadedLevelPath = LevelPath;
        MainMenuManager.Instance.StartEditor();
    }

    public void OpenDeleteWarning()
    {
        LevelHubManager.Instance.CurrentDeletingLevelCard = this;
        LevelHubManager.Instance.DeleteWarningPrompt.Tween.SetVisible(true);
        LevelHubManager.Instance.DeleteWarningBlockerTween.SetVisible(true);

        LevelHubManager.Instance.DeleteWarningPrompt.ConfirmationText.text = $"Do you really want to delete '{Name}'?";
    }

    public void DeleteLevel()
    {
        File.Delete(LevelPath);
        LevelListLoader.Instance.Refresh();
    }
}
