using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AnimationExtensions
{
    public static IEnumerator OnComplete(this Animation animation, string clipName, Action onComplete)
    {
        while (animation.IsPlaying(clipName))
            yield return null;

        onComplete();
    }
}

[RequireComponent(typeof(Animation))]
public class LetterAnimation : MonoBehaviour
{
    private Animation _animation;
    // Start is called before the first frame update

    public void PlayAppearing(Action onEnd = null)
    {
        string name = "LetterAppearing";
        _animation.Play(name);

        if (onEnd != null)
            StartCoroutine(name, onEnd);
    }

    public void PlayDisappearing(Action onEnd = null)
    {
        string name = "LetterDisappearing";
        _animation.Play(name);

        if (onEnd != null)
            StartCoroutine(_animation.OnComplete(name, onEnd));
    }

    void Start()
    {
        _animation = GetComponent<Animation>();
    }
}
