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
    private static readonly string IsPraying = "isPraying";
    private static readonly string IsListening = "isListening";
    private static readonly string IsDancing = "isDancing";
    private static readonly string IsMoving = "isMoving";
    private static readonly string TrDeath = "TrDeath";
    private static readonly string PreachRandomizer = "PreachRandomizer";
    private static readonly string DanceRandomizer = "DanceRandomizer";

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
    }

    private void OnEnable()
    {    
        GameEvents.Civilization.OnStartWalking += OnStartWalk;
        GameEvents.Civilization.OnStopWalking += OnStopWalk;
        GameEvents.Civilization.OnCivilizationDeath += OnTriggerDeath;
        GameEvents.Civilization.OnPreach += Preach;
        GameEvents.Civilization.OnPray += Pray;
        GameEvents.Civilization.OnListen += Listen;
    }

    private void OnDisable()
    {
        GameEvents.Civilization.OnStartWalking -= OnStartWalk;
        GameEvents.Civilization.OnStopWalking -= OnStopWalk;
        GameEvents.Civilization.OnCivilizationDeath -= OnTriggerDeath;
        GameEvents.Civilization.OnPreach -= Preach;
        GameEvents.Civilization.OnPray -= Pray;
        GameEvents.Civilization.OnListen -= Listen;
    }

    private void OnStartWalk(GameObject go)
    {
        if (go == gameObject || go == transform.parent.gameObject)
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
        if (go == gameObject || go == transform.parent.gameObject)
        {
            resetBools(); 
        }
    }
    
    
    public void SetIsDancing(bool isDancing)
    {
        resetBools();
        if (isDancing) mAnimator.SetInteger(DanceRandomizer, Random.Range(0, 2));
        mAnimator.SetBool(IsDancing, isDancing);
    }


    private void Preach(GameObject go)
    {
        if (go == transform.parent.gameObject)
        {
            StartCoroutine(Preach(15));
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
        SetIsPraying(true);
        Podium.SetActive(true);
        yield return new WaitForSecondsRealtime(10f);
        
        SetIsPreaching(true);
        float timer = 0;
        while (timer < duration)
        {
            mAnimator.SetInteger(PreachRandomizer, Random.Range(0, 4));
            timer += 2f;
            yield return new WaitForSecondsRealtime(2f);
        }
        GameEvents.Civilization.OnPreachEnd.Invoke(transform.parent.gameObject);
        SetIsPreaching(false);
        
        SetIsPraying(true);
        Podium.SetActive(true); 
        yield return new WaitForSecondsRealtime(5f);
        SetIsPraying(false);
    }
    
    private void Pray(GameObject go)
    {
        if (go == gameObject || go == transform.parent.gameObject)
        {
            SetIsPraying(true);
        }
    }
    public void SetIsPraying(bool isPraying)
    {
        resetBools();
        mAnimator.SetBool(IsPraying, isPraying);
    }

    private void Listen(GameObject go)
    {
        if (go == gameObject || go == transform.parent.gameObject)
        {
            resetBools();
            mAnimator.SetBool(IsListening, true);
        }
    }


    public void SetIsFishing(bool isFishing)
    {
        resetBools();
        mAnimator.SetBool(IsFishing, isFishing);
        FishingPole.SetActive(isFishing);
        Stool.SetActive(isFishing);
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
    
    
    private void OnTriggerDeath(GameObject civObject)
    {
        if (civObject == transform.parent.gameObject)
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
        yield return new WaitForSecondsRealtime(5);
        Destroy(transform.parent.gameObject);
    }

    private void resetBools()
    {
        mAnimator.SetBool(IsMoving, false);
        mAnimator.SetBool(IsDancing, false);
        mAnimator.SetBool(IsFarming, false);
        mAnimator.SetBool(IsTreeCutting, false);
        mAnimator.SetBool(IsPreaching, false);
        mAnimator.SetBool(IsPraying, false);
        mAnimator.SetBool(IsListening, false);
        mAnimator.SetBool(IsFishing, false);
        
        FishingPole.SetActive(false);
        Stool.SetActive(false);
        Podium.SetActive(false);
        Axe.SetActive(false);
        Hoe.SetActive(false);
        Pickaxe.SetActive(false);
    }
}
