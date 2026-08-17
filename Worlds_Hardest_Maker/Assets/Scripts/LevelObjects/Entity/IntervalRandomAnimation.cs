using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

/// <summary>
///     Triggers animation at random intervals
///     <para>Attach to gameObject holding animator</para>
/// </summary>
[RequireComponent(typeof(Animator))]
public class IntervalRandomAnimation : MonoBehaviour
{
    [FormerlySerializedAs("IntervalSeconds")] [SerializeField] [PositiveValueOnly] private float intervalSeconds;
    
    [FormerlySerializedAs("AnimTriggerString")] [SerializeField] private string animTriggerString;
    
    // value between 0 - 1, next trigger has to be in range of deviation
    [FormerlySerializedAs("LimitDeviation")] [Range(0, 1)] [SerializeField] private float limitDeviation;
    
    [FormerlySerializedAs("TriggerOnlyAtPlayMode")] [SerializeField] private bool triggerOnlyAtPlayMode;
    
    [FormerlySerializedAs("SoundEffect")] [SerializeField] [MustBeAssigned] private SoundEffect soundEffect;
    
    private int lastTrigger;
    
    private Animator anim;
    
    private IAudioService audioService;
    
    [Inject]
    private void Construct(IAudioService audioService)
    {
        this.audioService = audioService;
    }

    private void Awake() => anim = GetComponent<Animator>();
    
    private void FixedUpdate()
    {
        if (triggerOnlyAtPlayMode && !LevelSessionEditManager.Instance.IsPlaying) return;
        
        if (lastTrigger >= intervalSeconds / Time.fixedDeltaTime * limitDeviation) CheckAnimationTrigger();
        
        lastTrigger++;
    }
    
    private void CheckAnimationTrigger()
    {
        // check animation trigger
        float p = Time.fixedDeltaTime / intervalSeconds;
        
        if (Random.Range(0, 0.999f) >= p &&
            lastTrigger < intervalSeconds / Time.fixedDeltaTime * (limitDeviation + 1)) return;
        
        anim.SetTrigger(animTriggerString);
        
        audioService.Play(soundEffect);
        
        lastTrigger = 0;
    }
    
    public void Randomize() => lastTrigger = -(int)(Random.Range(0, intervalSeconds) / Time.fixedDeltaTime);
}