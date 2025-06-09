using System;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    private Animator mAnimator;
    private static readonly string IsMining = "isMining";
    private static readonly string IsFarming = "isFarming";
    private static readonly string IsTreeCutting = "isTreeCutting";
    private static readonly string IsFishing = "isFishing";
    private static readonly string IsPreaching = "isPreaching";
    private static readonly string IsDancing = "isDancing";
    private static readonly string IsMoving = "isMoving";
    private static readonly string TrDeath = "TrDeath";

    private GameObject FishingPole;
    private GameObject Stool;
    private GameObject Podium;
    private GameObject Axe;
    private GameObject Hoe;
    private GameObject Pickaxe;

    private void Start()
    {
        mAnimator = GetComponent<Animator>();
        FishingPole = transform.GetChild(2).gameObject;
        Stool = transform.GetChild(3).gameObject;
        Podium = transform.GetChild(4).gameObject;
        Axe = transform.GetChild(5).gameObject;
        Hoe = transform.GetChild(6).gameObject;
        Pickaxe = transform.GetChild(7).gameObject;
    }

    private void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                TriggerDeath();
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                bool isdoing = mAnimator.GetBool(IsMining);
                SetIsMining(!isdoing);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                bool isdoing = mAnimator.GetBool(IsFarming);
                SetIsFarming(!isdoing);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                bool isdoing = mAnimator.GetBool(IsTreeCutting);
                SetIsTreeCutting(!isdoing);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                bool isdoing = mAnimator.GetBool(IsFishing);
                SetIsFishing(!isdoing);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                bool isdoing = mAnimator.GetBool(IsPreaching);
                SetIsPreaching(!isdoing);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                bool isdoing = mAnimator.GetBool(IsDancing);
                SetIsDancing(!isdoing);
            }
        }
    }

    public void SetIsMoving(bool isMoving)
    {
        mAnimator.SetBool(IsMoving, isMoving);
    }
    
    public void SetIsDancing(bool isDancing)
    {
        resetBools();
        mAnimator.SetBool(IsDancing, isDancing);
    }
    
    public void SetIsMining(bool isMining)
    {
        resetBools();
        mAnimator.SetBool(IsMining, isMining);
        Pickaxe.SetActive(isMining);
    }
    
    public void SetIsFarming(bool isFarming)
    {
        resetBools();
        mAnimator.SetBool(IsFarming, isFarming);
        Hoe.SetActive(isFarming);
    }
    
    public void SetIsTreeCutting(bool isTreeCutting)
    {
        resetBools();
        mAnimator.SetBool(IsTreeCutting, isTreeCutting);
        Axe.SetActive(isTreeCutting);
    }
    
    public void SetIsPreaching(bool isPreaching)
    {
        resetBools();
        mAnimator.SetBool(IsPreaching, isPreaching);
        Podium.SetActive(isPreaching);
    }
    
    public void SetIsFishing(bool isFishing)
    {
        resetBools();
        mAnimator.SetBool(IsFishing, isFishing);
        FishingPole.SetActive(isFishing);
        Stool.SetActive(isFishing);
    }
    
    public void TriggerDeath()
    {
        resetBools();
        mAnimator.SetTrigger(TrDeath);
    }

    private void resetBools()
    {
        mAnimator.SetBool(IsMoving, false);
        mAnimator.SetBool(IsDancing, false);
        mAnimator.SetBool(IsMining, false);
        mAnimator.SetBool(IsFarming, false);
        mAnimator.SetBool(IsTreeCutting, false);
        mAnimator.SetBool(IsPreaching, false);
        mAnimator.SetBool(IsFishing, false);
        
        FishingPole.SetActive(false);
        Stool.SetActive(false);
        Podium.SetActive(false);
        Axe.SetActive(false);
        Hoe.SetActive(false);
        Pickaxe.SetActive(false);
    }
}
