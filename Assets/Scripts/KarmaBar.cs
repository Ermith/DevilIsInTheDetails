
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class KarmaBar : MonoBehaviour
{
    public UnityEngine.UI.Image GoodBar;
    public UnityEngine.UI.Image EvilBar;
    public TextMeshProUGUI Text;

    private float _textNumber = -1;

    public void SetKarma(float karma, float tweenTime = 1.2f)
    {
        if (_textNumber == -1)
            _textNumber = karma;

        GoodBar.DOFillAmount(MathF.Max(0, karma), tweenTime);
        EvilBar.DOFillAmount(MathF.Max(0, -karma), tweenTime);

        // tween the text
        DOTween.To(
            () => _textNumber,
            x =>
            {
                _textNumber = x;
                Text.text = $"{(int)(x * 1000)}";
            },
            karma,
            tweenTime
        );
    }

    public void OnKarmaChange()
    {
        SetKarma(GameDirector.GameDirectorInstance.Karma);
    }
}