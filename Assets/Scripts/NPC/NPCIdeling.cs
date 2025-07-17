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
        GameEvents.Civilization.OnPreach += onPreach; 
        GameEvents.Civilization.OnPreachEnd += onPreachEnd;
        GameEvents.Civilization.OnCivilizationMerge += OnCityMerge;
    }
    

    private void OnDisable()
    { 
        GameEvents.Civilization.OnCityFounded -= OnCityFounded;
        GameEvents.Civilization.OnPreach -= onPreach;
        GameEvents.Civilization.OnPreachEnd -= onPreachEnd;
        GameEvents.Civilization.OnCivilizationMerge -= OnCityMerge;
    }

    public void OnCityFounded(GameObject civObject)
    {
        if (civObject != gameObject) return;
        civ = civObject.GetComponent<Civilization>(); 
        var city = civ.GetComponent<Civilization>().city;
        housePos = ME.AdjustCoordsForHeight(city.GetHousePos());
        
        for (int i = transform.childCount; i > 1; i--)
        {
            var civiCiv = transform.GetChild(i - 1);
            var civiIdle= Instantiate(civiPrefab, civiCiv.position, civiCiv.rotation, transform);
            idles.Add(civiIdle);
            Destroy(civiCiv.gameObject);
        }
        EndIdleAction(gameObject);
    }

    public void OnCityMerge(GameObject arrivingCivObject, GameObject cityCivObject)
    {
        if (cityCivObject != gameObject) return;
        
        for (int i = arrivingCivObject.transform.childCount; i > 1; i--)
        {
            var civiCiv = arrivingCivObject.transform.GetChild(i - 1);
            var civiIdle= Instantiate(civiPrefab, civiCiv.position, civiCiv.rotation, transform);
            idles.Add(civiIdle);
        }
        
        Destroy(arrivingCivObject);
        EndIdleAction(gameObject);
    }
    
    IEnumerator SpawnAndGo(List<Vector3> destinations, Action<GameObject> onReached)
    {
        foreach (var des in destinations)
        {
            var civi= Instantiate(civiPrefab, housePos, Quaternion.identity, transform);
            idles.Add(civi);
            StartCoroutine(Walk(civi, ME.AdjustCoordsForHeight(des), onReached));
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator Walk(GameObject civi, Vector3 destination, Action<GameObject> onReached)
    {
        GameEvents.Civilization.OnStartWalking.Invoke(civi);
        
        var position = civi.transform.position;
        
        while (Vector3.Distance(position, destination) > 0.1f)
        {
            Vector3 direction = (destination - position).normalized;
            var newPos = position + direction * (10f * Time.deltaTime);
            civi.transform.position = ME.AdjustCoordsForHeight(newPos);
            civi.transform.rotation = Quaternion.LookRotation((newPos - position).normalized);
            position = newPos;
           
            yield return null;
        }
        
        GameEvents.Civilization.OnStopWalking.Invoke(civi);
        if (focusPoint != null) civi.transform.LookAt(focusPoint);
        onReached?.Invoke(civi);
    }
    
    private void EndIdleAction(GameObject gObject)
    {
        if (transform.position != gObject.transform.position) return;
        
        focusPoint = null;
        foreach (var civi in idles)
        {
            StartCoroutine(Walk(civi, housePos, Despawn));
        }
    }
    private void Despawn(GameObject civi)
    {
        idles.Remove(civi);
        Destroy(civi);
    }
    
    
    private void onPreach(GameObject saviour)
    {
        var saviourPos = saviour.transform.position;
        if (transform.position != saviourPos) return;

        focusPoint = saviour.transform;
        List<Vector3> audience = new();
        
        float distance = 2;
        int count = 0;
        for (var j = 0; j < 3; j++)
        {
            distance += 5;
            count += 4;
            for (var i=0; i<count; i++) 
            {
                if (audience.Count >= civ.population) goto end;
                
                var angle = Math.PI*2/count * i + Random.Range(-0.2f, 0.2f);
                float x = (float)(saviourPos.x + (distance+Random.Range(0f, 3f)) * Math.Cos(angle)) ;
                float z = (float)(saviourPos.z + (distance+Random.Range(0f, 3f)) * Math.Sin(angle));

                audience.Add(new Vector3(x, saviourPos.y, z));
            }
        }
        
        end:
        StartCoroutine(SpawnAndGo(audience, Listen));
        
    }
    private void Listen(GameObject go)
    {
        GameEvents.Civilization.OnListen.Invoke(go);
    }
    
    private void onPreachEnd(GameObject gObject) 
    {
        GameEvents.Civilization.OnPray.Invoke(gameObject);
        StartCoroutine(Pray(gObject));
    }
    IEnumerator Pray(GameObject gObject)
    {
        yield return new WaitForSeconds(5);
        EndIdleAction(gObject);
    }
}