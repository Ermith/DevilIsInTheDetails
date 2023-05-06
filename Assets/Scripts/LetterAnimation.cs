using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            StartCoroutine(WhatForEnd(name, onEnd));
    }

    public IEnumerator WhatForEnd(string name, Action onEnd)
    {
        while (_animation.IsPlaying(name))
            yield return null;

        onEnd();
    }

    void Start()
    {
        _animation = GetComponent<Animation>();
    }
}
