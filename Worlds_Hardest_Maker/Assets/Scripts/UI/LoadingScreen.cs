using System.Collections;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Separator("Settings")] [SerializeField]
    private float duration = 1;

    [Separator("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private ChainableTween tween;

    public void SetProgress(float progress) => slider.value = progress;

    public void LoadScene(int sceneId)
    {
        tween.Duration = duration;
        tween.StartChain();

        StartCoroutine(LoadSceneAsync(sceneId));
    }

    public void LoadScene(string sceneName)
    {
        Scene nextScene = SceneManager.GetSceneByName(sceneName);
        LoadScene(nextScene.buildIndex);
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        operation.allowSceneActivation = false;

        SetProgress(0);

        float elapsedTime = 0;

        float progressValue = 0;

        while (progressValue < 1 || elapsedTime < duration)
        {
            progressValue = Mathf.Clamp01(operation.progress / .9f);

            SetProgress(progressValue);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        operation.allowSceneActivation = true;
    }
}