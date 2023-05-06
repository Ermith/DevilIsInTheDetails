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
        _animation.Play("LetterCurse");
    }

    void Start()
    {
        _animation = GetComponent<Animation>();
        //PlayAppearing();
    }
}
