using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject civilisationPrefab;
    [SerializeField] public List<GameObject> civilisations;
    [SerializeField] private int civilisationCount = 4;
    private TileManager TM;

    private void Awake()
    {
        GameEvents.Civilization.OnCivilizationDeath += onCivDeath;
    }

    private void OnDisable()
    { 
        GameEvents.Civilization.OnCivilizationDeath -= onCivDeath;
    }

    private void Start()
    {
        TM = TileManager.Instance;

        for(int i=0; i< civilisationCount; i++)
        {
            int random = Random.Range(0, TM.spawnLocations.Count);
            Vector3 spawnLocation = TM.map.CellToWorld(TM.spawnLocations[random]);
            var civ = Instantiate(civilisationPrefab, spawnLocation, Quaternion.identity,transform);
            civilisations.Add(civ);
            civ.GetComponent<Civilization>().SetPopulation(Random.Range(1,8));
            StartCoroutine(WaitAndSettle(civ));
        }
    }
    
    IEnumerator WaitAndSettle(GameObject civ)
    {
        // suspend execution for 2 seconds
        yield return new WaitForSeconds(2);
        if (civ.GetComponent<Civilization>() != null) FindSettlingLocation(civ, 20);
    }
    
    private void FindSettlingLocation(GameObject civ, int range)
    {
        Vector3Int gridPos = TM.map.WorldToCell(civ.transform.position);
        List<Vector3Int> locations = TM.GetSpecificRange(gridPos, range);

        Vector3Int settlingPos = new Vector3Int();
        float winValue = 0;
        foreach(Vector3Int loc in locations)
        {
            float value = (TM.GetFood(loc) + TM.GetWater(loc) + TM.GetSafety(loc) + TM.GetShelter(loc) + TM.GetEnergy(loc)) / 5;
            if(winValue < value)
            {
                winValue = value;
                settlingPos = loc;
            }
        }
        civ.GetComponent<Civilization>().SetSettlingValues(settlingPos);
        
        civ.GetComponent<NPCMovement>().MovetoTileAndExecute(settlingPos, Settle);
    }
    
    private void Settle(GameObject civObject)
    {
        var civPos = TM.map.WorldToCell(civObject.transform.position);
        
        foreach (var civil in civilisations)
        {
            if (civil == civObject) continue;
            
            if (civil == null) continue;
            var civilPos = TM.map.WorldToCell(civil.transform.position);
            
            if (civPos == civilPos)
            {
                GameEvents.Civilization.OnCivilizationMerge.Invoke(civObject, civil);
                civilisations.Remove(civObject);
                Destroy(civObject);
                return;
            }
        }

        // Build city if no city is existent at location after movement
        if (civObject.TryGetComponent<Civilization>(out var civ))
        {
            if (civ.city == null)
            {
                civ.city = CityBuilder.Instance.BuildCity(civObject.transform.position, civObject.GetComponent<NPC>()._npcModel, civ);
                GameEvents.Civilization.OnCityFounded.Invoke(civObject);
            }
        }
        
    }

    private void onCivDeath(GameObject civObject)
    {
        civilisations.Remove(civObject);
        if (civilisations.Count == 0)
        {
            GameEvents.Lifecycle.OnGameEnd.Invoke();
        }
    }
}
