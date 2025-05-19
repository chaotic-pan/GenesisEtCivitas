using System;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    private Animator mAnimator;
    private void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                mAnimator.SetTrigger("TrDance");
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                mAnimator.SetTrigger("TrEnd");
            }
        }
    }
}
