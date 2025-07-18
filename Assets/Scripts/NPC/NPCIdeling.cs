using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

class NPCIdeling : MonoBehaviour
{
    public GameObject civiPrefab;

    private Civilization civ;
    private Vector3 housePos;
    private List<GameObject> idles = new();
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
            idles.Add(civi.gameObject);
        }
        SurroundFocusPoint(city._house.gameObject, StartBuild, 5);   
    }
    public void OnCityMerge(GameObject newCivObject, GameObject oldCivObject)
    {
        // reparent newCiv.civis to oldCiv
        if (oldCivObject != gameObject) return;
        
        for (int i = newCivObject.transform.childCount; i > 1; i--)
        { 
            var civi = newCivObject.transform.GetChild(i - 1);
            civi.SetParent(transform);
            idles.Add(civi.gameObject);
            StartCoroutine(Walk(civi.gameObject, housePos, Despawn));
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
    
    IEnumerator SpawnAndGo(List<Vector3> destinations, Action<GameObject> onReached)
    {
        for (int i = 0; i < idles.Count; i++)
        {
            if (i >= destinations.Count) yield break;
            var civi = idles[i];
            civi.SetActive(true);
            StartCoroutine(Walk(civi, ME.AdjustCoordsForHeight(destinations[i]), onReached));
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator Walk(GameObject civi, Vector3 destination, Action<GameObject> onReached)
    {
        GameEvents.Civilization.OnStartWalking?.Invoke(civi);
        
        var position = civi.transform.position;
        
        while (Vector3.Distance(position, destination) > 0.1f)
        {
            if (civi == null)
            {
                onReached?.Invoke(civi);
                yield break;
            }
            Vector3 direction = (destination - position).normalized;
            var newPos = position + direction * (10f * Time.deltaTime);
            civi.transform.position = ME.AdjustCoordsForHeight(newPos);
            civi.transform.rotation = Quaternion.LookRotation((newPos - position).normalized);
            position = newPos;
           
            yield return null;
        }
        
        GameEvents.Civilization.OnStopWalking?.Invoke(civi);
        if (focusPoint != null) civi.transform.LookAt(focusPoint);
        onReached?.Invoke(civi);
    }
    
    IEnumerator WaitAndEnd(GameObject gObject, float wait)
    {
        yield return new WaitForSeconds(wait);
        EndAllIdleAction(gameObject);
    }
    private void EndAllIdleAction(GameObject gObject)
    {
        if (gObject == null || transform.position != gObject.transform.position) return;
        
        focusPoint = null;
        foreach (var civi in idles)
        {
            StartCoroutine(Walk(civi, housePos, Despawn));
        }
        
        // var a= Instantiate(civiPrefab, housePos, Quaternion.identity, transform);
        // idles.Add(a);
        // StartCoroutine(Walk(a, ME.AdjustCoordsForHeight(new Vector3(housePos.x+10, housePos.y, housePos.z+10)), null));
    }
    private void Despawn(GameObject civi)
    {
        if (civi != null) civi.SetActive(false);
    }
    
    private void SurroundFocusPoint(GameObject focusObject,  Action<GameObject> onReached, float distance)
    {
        this.focusPoint = focusObject.transform;
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
        StartCoroutine(SpawnAndGo(audience, onReached));
    }
    
    
    private void OnPreach(GameObject saviour)
    {
        if (transform.position != saviour.transform.position) return;
        SurroundFocusPoint(saviour, Listen, 2);
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
        SurroundFocusPoint(building, StartBuild, 5);
    }
    private void StartBuild(GameObject civi)
    {
        GameEvents.Civilization.OnBuild.Invoke(civi);
        StartCoroutine(WaitAndEnd(civi,15));
    }
}