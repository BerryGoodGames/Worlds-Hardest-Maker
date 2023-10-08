using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;

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
    
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform levelCardContainer;

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
                levelInfo = levelInfo.OrderBy(x => x.LastAccessTime).ToArray();
                break;
        }

        if (IsDescending) Array.Reverse(levelInfo);

        if (levelsChanged || forceUpdateList)
        {
            UpdateLevelCards(levelInfo);
        }

        prevLevelInfo = levelInfo;
    }

    private void UpdateLevelCards(IEnumerable<FileInfo> levelInfo)
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

        foreach (FileInfo info in levelInfo)
        {
            // create new level cards
            LevelCardController levelCard = Instantiate(levelCardPrefab, levelCardContainer).GetComponent<LevelCardController>();

            // level card settings
            levelCard.Name = info.Name[..^4]; // remove .lvl at end
            levelCard.LevelPath = info.FullName;
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