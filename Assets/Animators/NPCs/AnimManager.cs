using System;
using System.Collections;
using Events;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimManager : MonoBehaviour
{
    private Animator mAnimator;
    private static readonly string IsFarming = "isFarming";
    private static readonly string IsTreeCutting = "isTreeCutting";
    private static readonly string IsFishing = "isFishing";
    private static readonly string IsPreaching = "isPreaching";
    private static readonly string IsDancing = "isDancing";
    private static readonly string IsMoving = "isMoving";
    private static readonly string TrDeath = "TrDeath";
    private static readonly string TrPreach = "TrPreach";

    private GameObject FishingPole;
    private GameObject Stool;
    private GameObject Podium;
    private GameObject Axe;
    private GameObject Hoe;
    private GameObject Pickaxe;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        FishingPole = transform.GetChild(2).gameObject;
        Stool = transform.GetChild(3).gameObject;
        Podium = transform.GetChild(4).gameObject;
        Axe = transform.GetChild(5).gameObject;
        Hoe = transform.GetChild(6).gameObject;
        Pickaxe = transform.GetChild(7).gameObject;
        
        GameEvents.Civilization.OnStartWalking += OnStartWalk;
        GameEvents.Civilization.OnStopWalking += OnStopWalk;
        GameEvents.Civilization.OnCivilizationDeath += OnTriggerDeath;
        GameEvents.Civilization.OnPreach += Preach;
    }

    private void OnDisable()
    {
        GameEvents.Civilization.OnStartWalking -= OnStartWalk;
        GameEvents.Civilization.OnStopWalking -= OnStopWalk;
        GameEvents.Civilization.OnCivilizationDeath -= OnTriggerDeath;
        GameEvents.Civilization.OnPreach -= Preach;
    }

    private void OnStartWalk(GameObject go)
    {
        if (go == transform.parent.gameObject)
        {
            resetBools();
            float randTime = Random.Range(1,50);
            StartCoroutine(delayedWalk(randTime/100f));
        }
    }
    
    IEnumerator delayedWalk(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        mAnimator.SetBool(IsMoving, true);
    }
    
    private void OnStopWalk(GameObject go)
    {
        if (go == transform.parent.gameObject)
        {
            resetBools(); 
        }
    }
    
    public void SetIsDancing(bool isDancing)
    {
        resetBools();
        mAnimator.SetBool(IsDancing, isDancing);
    }
    
    public void SetIsMining(bool isMining)
    {
        resetBools();
        mAnimator.SetBool(IsFarming, isMining);
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

    private void Preach(GameObject go, float duration)
    {
        if (go == transform.parent.gameObject)
        {
            StartCoroutine(Preach(duration));
            SetIsPreaching(true);
        }
    }

    public void SetIsPreaching(bool isPreaching)
    {
        resetBools();
        mAnimator.SetBool(IsPreaching, isPreaching);
        Podium.SetActive(isPreaching);
    }
    
    IEnumerator Preach(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            mAnimator.SetInteger("PreachRandomizer", Random.Range(0, 4));
            timer += 2f;
            yield return new WaitForSecondsRealtime(2f);
        }
        SetIsPreaching(false);
    }

    public void SetIsFishing(bool isFishing)
    {
        resetBools();
        mAnimator.SetBool(IsFishing, isFishing);
        FishingPole.SetActive(isFishing);
        Stool.SetActive(isFishing);
    }
    
    private void OnTriggerDeath(GameObject go)
    {
        if (go == transform.parent.gameObject)
        {
            resetBools();
            float randTime = Random.Range(1,50);
            transform.parent.GetComponent<NPCMovement>().StopAllCoroutines();
            StartCoroutine(delayedDeath(randTime/100f));
        }
    }
    IEnumerator delayedDeath(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        mAnimator.SetTrigger(TrDeath);
        yield return new WaitForSecondsRealtime(3);
        Destroy(transform.parent.gameObject);
    }

    private void resetBools()
    {
        mAnimator.SetBool(IsMoving, false);
        mAnimator.SetBool(IsDancing, false);
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
