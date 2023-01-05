using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuTween : MonoBehaviour
{
    [SerializeField] private Image cursor;
    [SerializeField] private Image player;
    [Space]
    [SerializeField] private float playerStartX = 530;
    [SerializeField] private float playerEndX = 238;
    [SerializeField] private float cursorStartAngle = 0;
    [SerializeField] private float cursorEndAngle = 12;
    [SerializeField] private Vector2 cursorStartPos = new(-172, 320);
    [SerializeField] private Vector2 cursorEndPos = new(275, -62);
    [Space] 
    [SerializeField] private float delay = 0;
    [SerializeField] private float cursorDelay = 0;
    [SerializeField] private float playerDuration = 1;
    [SerializeField] private float cursorDuration = 1;

    private void Start()
    {
        player.rectTransform.anchoredPosition = new(playerStartX, player.rectTransform.anchoredPosition.y);
        player.rectTransform.DOAnchorPosX(playerEndX, playerDuration)
            .SetEase(Ease.Linear)
            .SetDelay(delay);

        float cursorDelayTotal = cursorDelay + playerDuration + delay;
        cursor.rectTransform.eulerAngles = new(0, 0, cursorStartAngle);
        cursor.rectTransform.DORotate(new(0, 0, cursorEndAngle), cursorDuration)
            .SetDelay(cursorDelayTotal);
        
        cursor.rectTransform.anchoredPosition = cursorStartPos;
        cursor.rectTransform.DOAnchorPos(cursorEndPos, cursorDuration)
            .SetDelay(cursorDelayTotal);
    }
}
