using System.Collections;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class LoadingScreen : MonoBehaviour
{
    [Separator("Settings")] [SerializeField] [PositiveValueOnly] private float delay;
    
    [SerializeField] [PositiveValueOnly] private float duration = 1;
    
    [Separator("References")] [SerializeField] [InitializationField] [Required] private Slider slider;
    
    [SerializeField] [InitializationField] [Required] private ChainableTween tween;
    
    // [Inject] private ZenjectSceneLoader sceneLoader;
    
    private void SetProgress(float progress) => slider.value = progress;
    
    public void LoadScene(int sceneId)
    {
        tween.Delay = delay;
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
        yield return new WaitForSeconds(delay);
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId)!;
        
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