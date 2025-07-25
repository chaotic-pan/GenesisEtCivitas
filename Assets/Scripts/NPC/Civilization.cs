using System.Collections;
using System.Collections.Generic;
using CityStuff;
using DefaultNamespace;
using Events;
using UnityEngine;

public class Civilization : MonoBehaviour
{
    // base scores food, water, safety, shelter and energy go from 0 to 500
    // combined scores belief and happiness are calculated
    // ressources are gathered and go up to 500
    
    [SerializeField] public string civilisationName;
    [SerializeField] private float food = 250;
    [SerializeField] private float water = 250;
    [SerializeField] private float safety = 250;
    [SerializeField] private float shelter = 250;
    [SerializeField] private float energy = 250;
    private TileManager TM = TileManager.Instance;

    public int population;
    private float belief;
    private float happiness;

    private float ressources;
    public Language Language;

    public City city;

    public GameObject civiPrefab;

    private readonly List<Vector3> _NPCPoints = new()
    {
        new Vector3(-0.33f, 0f, -0.36f),
        new Vector3(0.42f, 0f, -0.36f),
        new Vector3(0.06f, 0f, -0.75f),
        new Vector3(-0.5f, 0f, -0.9f),
        new Vector3(0.63f, 0f, -0.9f),
        new Vector3(-0.18f, 0f, -1.3f),
        new Vector3(0.3f, 0f, -1.31f),
        new Vector3(-0.33f, 0f, 0.36f),
        new Vector3(0.42f, 0f, 0.36f),
        new Vector3(0.06f, 0f, 0.75f),
        new Vector3(-0.5f, 0f, 0.9f),
        new Vector3(0.63f, 0f, 0.9f),
        new Vector3(-0.18f, 0f, 1.3f),
        new Vector3(0.3f, 0f, 1.31f),
    };

    private void Awake()
    {
        GameEvents.Civilization.OnCivilizationMerge += MergeCivilisation;
        StartCoroutine(TileStatsDecay(10));
    }

    private void OnDisable()
    {
        GameEvents.Civilization.OnCivilizationMerge -= MergeCivilisation;
    }

    public void Initialize()
    {
        Language = new Language();
    }
    
    public void SetPopulation(int population)
    {
        this.population = population;
        GetComponent<NPC>()._npcModel.Population = population;
        spawnCivis(population);
    }

    public void spawnCivis(int count)
    {
        var civPopDif = (transform.childCount-1) - count;
        
        switch (civPopDif)
        {
            case > 0:
            {
                for (int i = transform.childCount; i > count+1; i--)
                {
                    Destroy(transform.GetChild(i-1).gameObject);
                }

                break;
            }
            case < 0:
            {
                for (int i = 0; i < -civPopDif; i++)
                {
                    var civi = Instantiate(civiPrefab, Vector3.zero, Quaternion.identity, transform);
                    civi.transform.localPosition = _NPCPoints[i];
                }

                break;
            }
        }
    }

    IEnumerator TileStatsDecay(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (city != null)
        {
            GameEvents.Civilization.OnTileStatsDecay.Invoke(gameObject);
        }
        StartCoroutine(TileStatsDecay(timer));
    }

    public void SetSettlingValues(Vector3Int vec)
    {
        food = TM.GetFood(vec);
        water = TM.GetWater(vec);
        safety = TM.GetSafety(vec);
        shelter = TM.GetShelter(vec);
        energy = TM.GetEnergy(vec);

        CalcValues();
        if (TryGetComponent<NPC>(out var npc)) npc.UpdateValues();
    }

    public void CalcValues()
    {
        belief = (food + water + safety + shelter + energy) / 5; //TODO: include churches, actions by player etc. into this calculation
        happiness = (food + water + safety + shelter + energy) / 5 + (ressources / 5);
        ressources = 250; //TODO: adjust ressources dependant on tiles
    }

    private void MergeCivilisation(GameObject civAObject, GameObject civBObject)
    {
        if (gameObject == civBObject)
        {
            var newPop = civAObject.GetComponent<Civilization>().population;
            
            population +=newPop;
            GetComponent<NPC>()._npcModel.Population = population;
        }
    }
    
    public void OnMouseEnter()
    {
        if (city == null) return;
        city.OnMouseEnter();
    }

    public float Food
    {
        get => food;
        set
        {
            food = value;
        }
    }
    public float Water
    {
        get => water;
        set
        {
            water = value;
        }
    }
    public float Safety
    {
        get => safety;
        set
        {
            safety = value;
        }
    }
    public float Shelter
    {
        get => shelter;
        set
        {
            shelter = value;
        }
    }
    public float Energy
    {
        get => energy;
        set
        {
            energy = value;
        }
    }
    public float Belief
    {
        get => belief;
        set
        {
            belief = value;
        }
    }
}
