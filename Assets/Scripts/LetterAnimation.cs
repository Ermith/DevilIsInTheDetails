using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LetterAnimation : MonoBehaviour
{
    private Animation _animation;
    // Start is called before the first frame update

    public void PlayAppearing()
    {
        _animation.Play("Letter Appearing");
    }

    void Start()
    {
        _animation = GetComponent<Animation>();
        //PlayAppearing();
    }
}
