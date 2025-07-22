using System.Collections;
using System.Collections.Generic;
using CityStuff;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject civilisationPrefab;
    [SerializeField] public List<GameObject> civilisations;
    [SerializeField] private int civilisationCount = 4;
    
    public List<City> cities;
    private TileManager TM;

    private void Awake()
    {
        GameEvents.Civilization.OnCivilizationDeath += onCivDeath;
        GameEvents.Civilization.OnCivilizationLowOnStats += SplitCiv;
    }

    private void OnDisable()
    { 
        GameEvents.Civilization.OnCivilizationDeath -= onCivDeath;
        GameEvents.Civilization.OnCivilizationLowOnStats += SplitCiv;
    }

    private void Start()
    {
        
        print("A");
        StartCoroutine(Spawning());
    }

    IEnumerator Spawning()
    {
        TM = TileManager.Instance;
        for(int i=0; i< civilisationCount; i++)
        {
            int random = Random.Range(0, TM.spawnLocations.Count);
            while (TM.IsOcean(TM.spawnLocations[random]))
            {
                random = Random.Range(0, TM.spawnLocations.Count);
            }
            
            Vector3 spawnLocation = TM.CellToWorld(TM.spawnLocations[random]);
            var population = Random.Range(1, 8);
            
            SpawnCiv(spawnLocation, population, 20, 0);
        }
        print("Z");
        yield break;
    }

    private GameObject SpawnCiv(Vector3 location, int population, int settleRangeIncl, int settleRangeExcl)
    {
        var civ = Instantiate(civilisationPrefab, location, Quaternion.identity,transform);
        civilisations.Add(civ);
        civ.GetComponent<Civilization>().SetPopulation(population);
        StartCoroutine(WaitAndSettle(civ, settleRangeIncl, settleRangeExcl));
        return civ;
    }
    
    IEnumerator WaitAndSettle(GameObject civ, int rangeIncl, int rangeExcl)
    {
        // suspend execution for 2 seconds
        yield return new WaitForSeconds(2);
        if (civ.GetComponent<Civilization>() != null) FindSettlingLocation(civ, rangeIncl, rangeExcl);
    }
    
    private void FindSettlingLocation(GameObject civ, int range, int excludedRange)
    {
        Vector3Int gridPos = TM.WorldToCell(civ.transform.position);

        List<Vector3Int> excludedLocations = TM.GetSpecificRange(gridPos, excludedRange);
        List<Vector3Int> locations = TM.GetSpecificRange(gridPos, range);
        foreach (Vector3Int item in excludedLocations)
        {
            locations.Remove(item);
        }

        Vector3Int settlingPos = new Vector3Int();
        float winValue = 0;
        foreach(Vector3Int loc in locations)
        {
            if (TM.IsOcean(loc))
            {
                continue;
            }
            var surrounding = TM.GetSpecificRange(loc, 2);
            foreach (var neighbour in surrounding)
            {
                if (TM.IsOcean(neighbour))
                {
                    goto cont;
                }
            }
            
            float value = (TM.GetFood(loc) + TM.GetWater(loc) + TM.GetSafety(loc) + TM.GetShelter(loc) + TM.GetEnergy(loc)) / 5;
            if(winValue < value)
            {
                winValue = value;
                settlingPos = loc;
            }

            cont: ;
        }
        civ.GetComponent<Civilization>().SetSettlingValues(settlingPos);
        
        civ.GetComponent<NPCMovement>().MovetoTileInRangeAndExecute(settlingPos, TM.GetSpecificRange(gridPos, range), Settle);
    }
    
    private void Settle(GameObject civObject)
    {
        var civPos = TM.WorldToCell(civObject.transform.position);
        
        // check if there is already another city in this area
        var tileDistance = 3; //min amount of tiles between two cities

        City InvalidCity = null;
        
        foreach (var city in cities)
        {
            if (city == null)
            {
                InvalidCity = city;
                continue;
            }
            
            var cityPos = TM.WorldToCell(city.transform.position);
            
            // if other city found, merge
            if (civPos == cityPos && city.civ != null)
            {
                GameEvents.Civilization.OnCivilizationMerge.Invoke(civObject, city.civ.gameObject);
                civilisations.Remove(civObject);
                return;
            }
            // if another city is in rage, go there
            if (TM.GetTileDistance(civPos, cityPos) <= tileDistance)
            {
                civObject.GetComponent<NPCMovement>().MovetoTileAndExecute(cityPos, Settle);
                return;
            }
        }

        if (InvalidCity != null) cities.Remove(InvalidCity);


        // if no other city found, build new city
        if (civObject.TryGetComponent<Civilization>(out var civ) && civ.city == null)
        {
            civ.city = CityBuilder.Instance.BuildCity(civObject.transform.position, civObject.GetComponent<NPC>()._npcModel, civ);
            cities.Add(civ.city);
            GameEvents.Civilization.OnCityFounded.Invoke(civObject);
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

    private void SplitCiv(GameObject civObject)
    { 
        Civilization civ = civObject.GetComponent<Civilization>();
        if (civ.population <= 3 || civ.city == null) return;
        
        civ.SetPopulation(civ.population/2);
        var newCiv = SpawnCiv(civObject.transform.position, civ.population, 25, 5);
        
        GameEvents.Civilization.OnCivilizationSplit.Invoke(newCiv, civObject);
    }
}
