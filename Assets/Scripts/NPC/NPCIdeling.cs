using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

class NPCIdeling : MonoBehaviour
{
    private enum IdleState
    {
        NaN, Wandering, Building, Praying
    }
    
    private Civilization civ;
    private Vector3 housePos;
    private Dictionary<GameObject, IdleState> idles = new();
    private Transform focusPoint;
    private MapExtractor ME = MapExtractor.Instance;
    
    private void OnEnable()
    { 
        GameEvents.Civilization.OnCityFounded += OnCityFounded;
        GameEvents.Civilization.OnPreach += OnPreach; 
        GameEvents.Civilization.OnPreachEnd += OnPreachEnd;
        GameEvents.Civilization.OnCivilizationMerge += OnCityMerge;
        GameEvents.Civilization.OnCivilizationSplit += OnCivSplit;
        GameEvents.Civilization.CreateBuilding += OnCreateBuilding;
    }
    private void OnDisable()
    { 
        GameEvents.Civilization.OnCityFounded -= OnCityFounded;
        GameEvents.Civilization.OnPreach -= OnPreach;
        GameEvents.Civilization.OnPreachEnd -= OnPreachEnd;
        GameEvents.Civilization.OnCivilizationMerge -= OnCityMerge;
        GameEvents.Civilization.OnCivilizationSplit -= OnCivSplit;
        GameEvents.Civilization.CreateBuilding -= OnCreateBuilding;
    }

    public void OnCityFounded(GameObject civObject)
    {
        if (civObject != gameObject) return;
        civ = civObject.GetComponent<Civilization>(); 
        var city = civ.GetComponent<Civilization>().city;
        housePos = ME.AdjustCoordsForHeight(city.GetHousePos());
        
        for (int i = transform.childCount; i > 1; i--)
        {
            var civi = transform.GetChild(i - 1);
            idles.Add(civi.gameObject, IdleState.NaN);
        }
        SurroundFocusPoint(city._house.gameObject, StartBuild, 5, IdleState.Building);   
    }
    public void OnCityMerge(GameObject newCivObject, GameObject oldCivObject)
    {
        // reparent newCiv.civis to oldCiv
        if (oldCivObject != gameObject) return;
        
        for (int i = newCivObject.transform.childCount; i > 1; i--)
        { 
            var civi = newCivObject.transform.GetChild(i - 1);
            civi.SetParent(transform);
            idles.Add(civi.gameObject, IdleState.NaN);
            EndIdleAction(civi.gameObject);
        }
        
        Destroy(newCivObject);
    }
    public void OnCivSplit(GameObject newCivObject, GameObject oldCivObject)
    {
        if (oldCivObject != gameObject) return;
        
        for (int i = newCivObject.transform.childCount; i > 1; i--)
        { 
            var civi = oldCivObject.transform.GetChild(oldCivObject.transform.childCount - 1).gameObject;
            idles.Remove(civi);
            // Destroy(civi);
        }
    }
    
    IEnumerator SpawnAndGo(List<Vector3> destinations, Action<GameObject> onReached, IdleState state)
    {
        for (int i = 0; i < idles.Count; i++)
        {
            if (i >= destinations.Count) yield break;
            var civi = idles.ElementAt(i).Key;
            if (civi == null) continue;
            civi.SetActive(true);
            
            var switchChance =  idles[civi] switch
            {
                IdleState.Building => 50,
                IdleState.Praying => 20,
                _ => 100
            };
            var random = Random.Range(0, 100);
            
            if (switchChance >= random)
            {
                idles[civi] = state;
                StartCoroutine(Walk(civi, ME.AdjustCoordsForHeight(destinations[i]), onReached));
            }

            yield return new WaitForSeconds(0.5f);         
             
        }
    }
    IEnumerator Walk(GameObject civi, Vector3 destination, Action<GameObject> onReached)
    {
        if (civi == null) yield break;
        
        GameEvents.Civilization.OnStartWalking?.Invoke(civi);
        var position = civi.transform.position;
        var swim = false;
        
        while (Vector3.Distance(position, destination) > 0.1f)
        {
            if (civi == null) yield break;
            
            Vector3 direction = (destination - position).normalized;
            var newPos = position + direction * (10f * Time.deltaTime);
            civi.transform.position = ME.AdjustCoordsForHeight(newPos);
            civi.transform.rotation = Quaternion.LookRotation((newPos - position).normalized);
            position = newPos;
            if (swim == ME.IsWalkable(newPos))
            {
                swim = !swim;
                GameEvents.Civilization.OnSwim?.Invoke(civi, swim);
            }

            yield return null;
        }
        
        GameEvents.Civilization.OnStopWalking?.Invoke(civi);
        if (focusPoint != null) civi.transform.LookAt(focusPoint);
        onReached?.Invoke(civi);
    }
    
    IEnumerator WaitAndEnd(GameObject civiObject, float wait)
    {
        yield return new WaitForSeconds(wait);
        EndIdleAction(civiObject);
    }
    private void EndIdleAction(GameObject civi)
    {
        if (civi == null) return;
        
        focusPoint = null;

        var i = idles.Keys.ToList().IndexOf(civi);
        var chance = Random.Range(0, 100);
        if (i == 0 || 50 >= chance)
        {
            idles[civi] = IdleState.Wandering;
            var wanderPos = GetWanderPos(civi);
            StartCoroutine(Walk(civi, wanderPos, Wandering));
        }
        else
        {
            idles[civi] = IdleState.NaN;
            StartCoroutine(Walk(civi, housePos, Despawn));
        }
            
    }
    private void Despawn(GameObject civi)
    {
        if (civi != null) civi.SetActive(false);
    }
    
    private void SurroundFocusPoint(GameObject focusObject,  Action<GameObject> onReached, float distance, IdleState state)
    {
        focusPoint = focusObject.transform;
        var focusPos = focusObject.transform.position;
        
        List<Vector3> audience = new();
        
        int count = 0;
        for (var j = 0; j < 3; j++)
        {
            distance += 5;
            count += 4;
            for (var i=0; i<count; i++) 
            {
                if (audience.Count >= idles.Count) goto end;
                
                var angle = Math.PI*2/count * i + Random.Range(-0.2f, 0.2f) + Math.PI/8*j;
                float x = (float)(focusPos.x + (distance+Random.Range(0f, 3f)) * Math.Cos(angle)) ;
                float z = (float)(focusPos.z + (distance+Random.Range(0f, 3f)) * Math.Sin(angle));

                audience.Add(new Vector3(x, focusPos.y, z));
            }
        }
        
        end:
        StopAllCoroutines();
        StartCoroutine(SpawnAndGo(audience, onReached, state));
    }
    
    
    private void OnPreach(GameObject saviour)
    {
        if (transform.position != saviour.transform.position) return;
        SurroundFocusPoint(saviour, Listen, 2, IdleState.Praying);
    }
    private void Listen(GameObject go)
    {
        GameEvents.Civilization.OnListen.Invoke(go);
    }
    
    private void OnPreachEnd(GameObject gObject) 
    {
        GameEvents.Civilization.OnPray.Invoke(gameObject);
        StartCoroutine(WaitAndEnd(gObject, 5));
    }

    private void OnCreateBuilding(GameObject civObject, GameObject building)
    {
        if (civObject != gameObject) return;
        SurroundFocusPoint(building, StartBuild, 5, IdleState.Building);
    }
    private void StartBuild(GameObject civi)
    {
        GameEvents.Civilization.OnBuild?.Invoke(civi);
        StartCoroutine(WaitAndEnd(civi,15));
    }

    private Vector3 GetWanderPos(GameObject civi)
    {
        if (this == null) return Vector3.zero;
        var cityPos = transform.position;

        var x = cityPos.x + Random.Range(-100, 100);
        var z = cityPos.z + Random.Range(-100, 100);

        return ME.AdjustCoordsForHeight(new Vector3(x, 0, z));
    }
    private async void Wandering(GameObject civi)
    {
        if (ME.IsWalkable(civi.transform.position))
        {
            var wait = Random.Range(1, 10);
            await Task.Delay(wait *1000);
        }
        var wanderPos = GetWanderPos(civi);
        StartCoroutine(Walk(civi, wanderPos, Wandering));
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}