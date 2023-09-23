using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTween : MonoBehaviour
{
    [SerializeField] private Image cursor;
    [SerializeField] private Image player;
    [Space] [SerializeField] private float playerStartX = 530;
    [SerializeField] private float playerEndX = 238;
    [SerializeField] private float cursorStartAngle;
    [SerializeField] private float cursorEndAngle = 12;
    [SerializeField] private Vector2 cursorStartPos = new(-172, 320);
    [SerializeField] private Vector2 cursorEndPos = new(275, -62);
    [Space] [SerializeField] private float delay;
    [SerializeField] private float cursorDelay;
    [SerializeField] private float playerDuration = 1;
    [SerializeField] private float cursorDuration = 1;

    private void Start()
    {
        player.rectTransform.anchoredPosition = new(playerStartX, player.rectTransform.anchoredPosition.y);
        player.rectTransform.DOAnchorPosX(playerEndX, playerDuration)
            .SetEase(Ease.Linear)
            .SetDelay(delay)
            .SetId(gameObject);

        float cursorDelayTotal = cursorDelay + playerDuration + delay;
        cursor.rectTransform.eulerAngles = new(0, 0, cursorStartAngle);
        cursor.rectTransform.DORotate(new(0, 0, cursorEndAngle), cursorDuration)
            .SetDelay(cursorDelayTotal)
            .SetId(gameObject);

        cursor.rectTransform.anchoredPosition = cursorStartPos;
        cursor.rectTransform.DOAnchorPos(cursorEndPos, cursorDuration)
            .SetDelay(cursorDelayTotal)
            .SetEase(Ease.OutQuint)
            .SetId(gameObject);
    }

    private void OnDestroy() => DOTween.Kill(gameObject);
}