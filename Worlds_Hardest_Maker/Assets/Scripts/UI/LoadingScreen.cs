using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject loadingScreen;

    public void SetProgress(float progress) => slider.value = progress;

    public void LoadScene(int sceneId)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    public void LoadScene(string sceneName)
    {
        // TODO: fix
        Scene nextScene = SceneManager.GetSceneByName(sceneName);
        LoadScene(nextScene.buildIndex);
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        SetProgress(0);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / .9f);

            SetProgress(progressValue);

            yield return null;
        }
    }
}