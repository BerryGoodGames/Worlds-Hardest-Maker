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

    [Separator("References")] [SerializeField]
    private GameObject levelCardPrefab;

    [SerializeField] private TMP_Dropdown sortInput;

    private FileInfo[] prevLevelInfo;

    private bool refreshScheduled;

    [HideInInspector] public SortSettings SortSetting = SortSettings.Name;

    private Dictionary<string, SortSettings> stringToSetting = new()
    {
        { "Name", SortSettings.Name },
        { "Latest", SortSettings.Latest }
    };

    private void Awake()
    {
        StartCoroutine(LoadCoroutine());

        Instance ??= this;
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
    public void Refresh()
    {
        if (refreshScheduled) return;

        StartCoroutine(ScheduledRefresh());

        refreshScheduled = true;
    }

    private IEnumerator ScheduledRefresh()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
            yield return new WaitForEndOfFrame();

        refreshScheduled = false;

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

        if (levelsChanged)
        {
            UpdateLevelCards(levelInfo);
        }

        prevLevelInfo = levelInfo;
    }

    private void UpdateLevelCards(IEnumerable<FileInfo> levelInfo)
    {
        // destroy all level cards
        foreach (Transform t in transform)
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
            LevelCardController levelCard = Instantiate(levelCardPrefab, transform).GetComponent<LevelCardController>();

            // level card settings
            levelCard.Name = info.Name[..^4]; // remove .lvl at end
            levelCard.LevelPath = info.FullName;
        }
    }

    public void SetSortSetting(int index)
    {
        SortSetting = stringToSetting[sortInput.options[index].text];

        Refresh();
    }

    private void OnDestroy()
    {
        print("Destroy");
    }
}

public enum SortSettings
{
    Name,
    Latest
}