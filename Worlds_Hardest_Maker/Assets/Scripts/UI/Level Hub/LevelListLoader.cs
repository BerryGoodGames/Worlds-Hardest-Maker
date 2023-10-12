using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelListLoader : MonoBehaviour
{
    public static LevelListLoader Instance { get; private set; }

    [SerializeField] private bool refresh;

    [SerializeField] [ConditionalField(nameof(refresh))]
    private float refreshInterval = 5;

    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned]
    private GameObject levelCardPrefab;

    [SerializeField][InitializationField][MustBeAssigned] private TMP_Dropdown sortInput;
    [SerializeField][InitializationField][MustBeAssigned] private ButtonVerticalArrowTween sortOrderButton;

    [SerializeField][InitializationField][MustBeAssigned] private Transform levelCardContainer;
    [InitializationField][MustBeAssigned] public ContentSizeFitter LevelCardContentSizeFitter;

    private FileInfo[] prevLevelInfo;

    [HideInInspector] public SortSettings SortSetting = SortSettings.Name;
    [HideInInspector] public bool IsDescending;

    private readonly Dictionary<string, SortSettings> stringToSetting = new()
    {
        { "Latest", SortSettings.Latest },
        { "Name", SortSettings.Name }
    };

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    private void Start()
    {
        UpdateSortSetting();
        UpdateSortOrder();

        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        while (true)
        {
            if (refresh) Refresh();

            // wait until trying to load levels again
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    [ButtonMethod]
    public void Refresh() => Refresh(false);

    public void Refresh(bool forceUpdateList)
    {
        DirectoryInfo levelDirectory = new(SaveSystem.LevelSavePath);

        FileInfo[] levelInfo = levelDirectory.GetFiles("*.lvl");
        
        bool levelsChanged = false;

        if (prevLevelInfo == null || levelInfo.Length != prevLevelInfo.Length) levelsChanged = true;
        else
        {
            levelInfo = levelInfo.OrderBy(x => x.Name).ToArray();
            prevLevelInfo = prevLevelInfo.OrderBy(x => x.Name).ToArray();

            for (int i = 0; i < levelInfo.Length; i++)
            {
                if (levelInfo[i].Name == prevLevelInfo[i].Name) continue;
                levelsChanged = true;
                break;
            }
        }

        // sort level info
        switch (SortSetting)
        {
            case SortSettings.Name:
                levelInfo = levelInfo.OrderBy(x => x.Name).ToArray();
                break;
            case SortSettings.Latest:
                levelInfo = levelInfo.OrderBy(x => x.LastAccessTime).Reverse().ToArray();
                break;
        }

        if (IsDescending) Array.Reverse(levelInfo);

        if (levelsChanged || forceUpdateList)
        {
            UpdateLevelCards(levelInfo);
        }

        prevLevelInfo = levelInfo;
    }

    private void UpdateLevelCards(FileInfo[] levelInfo)
    {
        // destroy all level cards
        foreach (Transform t in levelCardContainer)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(t.gameObject);
                continue;
            }
#endif

            Destroy(t.gameObject);
        }

        // load all levels into LevelData in array
        LevelData[] levelDataArr = new LevelData[levelInfo.Length];
        for (int i = 0; i < levelInfo.Length; i++)
        {
            try
            {
                levelDataArr[i] = SaveSystem.LoadLevel(levelInfo[i].FullName);
            }
            catch (Exception)
            {
                // failed to load file -> old / corrupt file
                // ignored
            }
        }

        // create level cards and display info
        for (int i = 0; i < levelDataArr.Length; i++)
        {
            if (levelDataArr[i] == null) continue;

            LevelData levelData = levelDataArr[i];
            FileInfo levelFileInfo = levelInfo[i];

            LevelInfo info = levelData.Info;

            // create new level cards
            LevelCardController levelCard = Instantiate(levelCardPrefab, levelCardContainer).GetComponent<LevelCardController>();

            // level card settings
            levelCard.Name = levelFileInfo.Name[..^4]; // removes .lvl at end
            levelCard.Creator = $"by {info.Creator}";
            levelCard.Description = info.Description;
            levelCard.LastEdited = $"Last edited {info.LastEdited}";
            levelCard.EditTime = $@"Edit time: {info.EditTime:hh\:mm\:ss}";
            levelCard.LevelPath = levelFileInfo.FullName;
        }
    }

    public void UpdateSortSetting() => SortSetting = stringToSetting[sortInput.options[sortInput.value].text];

    public void UpdateSortOrder() => IsDescending = sortOrderButton.IsUp;
}

public enum SortSettings
{
    Name,
    Latest
}