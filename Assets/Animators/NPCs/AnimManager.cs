using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimManager : MonoBehaviour
{
    private Animator mAnimator;
    private static readonly string IsFarming = "isFarming";
    private static readonly string IsTreeCutting = "isTreeCutting";
    private static readonly string IsFishing = "isFishing";
    private static readonly string IsBuilding = "isBuilding";
    private static readonly string IsPreaching = "isPreaching";
    private static readonly string IsPraying = "isPraying";
    private static readonly string IsListening = "isListening";
    private static readonly string IsDancing = "isDancing";
    private static readonly string IsMoving = "isMoving";
    private static readonly string Water = "Water";
    private static readonly string TrDeath = "TrDeath";
    private static readonly string PreachRandomizer = "PreachRandomizer";
    private static readonly string DanceRandomizer = "DanceRandomizer";

    private GameObject FishingPole;
    private GameObject Stool;
    private GameObject Podium;
    private GameObject Axe;
    private GameObject Hoe;
    private GameObject Hammer;
    private GameObject Pickaxe;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        FishingPole = transform.GetChild(2).gameObject;
        Stool = transform.GetChild(3).gameObject;
        Podium = transform.GetChild(4).gameObject;
        Axe = transform.GetChild(5).gameObject;
        Hoe = transform.GetChild(6).gameObject;
        Hammer = transform.GetChild(7).gameObject;
        Pickaxe = transform.GetChild(8).gameObject;
    }

    private void OnEnable()
    {    
        GameEvents.Civilization.OnStartWalking += OnStartWalk;
        GameEvents.Civilization.OnStopWalking += OnStopWalk;
        GameEvents.Civilization.OnSwim += Swim;
        GameEvents.Civilization.OnCivilizationDeath += OnTriggerDeath;
        GameEvents.Civilization.OnPreach += Preach;
        GameEvents.Civilization.OnPray += Pray;
        GameEvents.Civilization.OnListen += Listen;
        GameEvents.Civilization.OnBuild += Build;
    }

    private void OnDisable()
    {
        GameEvents.Civilization.OnStartWalking -= OnStartWalk;
        GameEvents.Civilization.OnStopWalking -= OnStopWalk;
        GameEvents.Civilization.OnSwim -= Swim;
        GameEvents.Civilization.OnCivilizationDeath -= OnTriggerDeath;
        GameEvents.Civilization.OnPreach -= Preach;
        GameEvents.Civilization.OnPray -= Pray;
        GameEvents.Civilization.OnListen -= Listen;
        GameEvents.Civilization.OnBuild -= Build;
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

    private void Swim(GameObject go, bool swim)
    {
        if (go == gameObject || go == transform.parent.gameObject)
        {
            mAnimator.SetFloat(Water, swim? 1 : 0, 0.2f, Time.deltaTime);
        }
    }
    
    
    public void SetIsDancing(bool isDancing)
    {
        if (isDancing) mAnimator.SetInteger(DanceRandomizer, Random.Range(0, 2));
        setBool(IsDancing, null, isDancing);
    }


    private void Preach(GameObject go)
    {
        if (go == transform.parent.gameObject)
        {
            StartCoroutine(Preach(15));
        }
    }
    IEnumerator Preach(float duration)
    {
        setBool(IsPraying, new List<GameObject>{Podium}, true);
        yield return new WaitForSecondsRealtime(10f);
        
        setBool(IsPreaching, new List<GameObject>{Podium}, true);
        float timer = 0;
        while (timer < duration)
        {
            mAnimator.SetInteger(PreachRandomizer, Random.Range(0, 4));
            timer += 2f;
            yield return new WaitForSecondsRealtime(2f);
        }
        GameEvents.Civilization.OnPreachEnd.Invoke(transform.parent.gameObject);
        setBool(IsPreaching, null, false);
        
        setBool(IsPraying, new List<GameObject>{Podium}, true);
        yield return new WaitForSecondsRealtime(5f);
        setBool(IsPraying, null, false);
    }
    
    private void Pray(GameObject go)
    {
        if (go == gameObject || go == transform.parent.gameObject)
        {
            setBool(IsPraying, null, true);
        }
    }

    private void Listen(GameObject go)
    {
        if (go == gameObject || go == transform.parent.gameObject)
        {
            resetBools();
            setBool(IsListening, null, true);
        }
    }
    
    
    private void Build(GameObject go)
    {
        if (go == gameObject || go == transform.parent.gameObject)
        {
            StartCoroutine(Build(10));
        }
    }
    IEnumerator Build(float duration)
    {
        setBool(IsBuilding, new List<GameObject>{Hammer}, true);
        print("AYYYYY! I'm buildin' here!");
        yield return new WaitForSecondsRealtime(duration);
        setBool(IsBuilding, null, false);
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


    private void setBool(string name, List<GameObject> activeProbs, bool isActive)
    {
        resetBools();
        mAnimator.SetBool(name, isActive);
        if (activeProbs == null) return;
        foreach (var prob in activeProbs)
        {
            prob.SetActive(isActive);
        }
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
        Hammer.SetActive(false);
        Pickaxe.SetActive(false);
    }
}
