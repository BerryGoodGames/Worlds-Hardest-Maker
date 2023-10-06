using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        LevelHubManager.LoadLevelPath = LevelPath;

        SceneManager.LoadScene("DefaultEditorScene");
    }
}
