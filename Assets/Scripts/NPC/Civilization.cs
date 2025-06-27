using CityStuff;
using DefaultNamespace;
using Unity.VisualScripting;
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

    public float population;
    private float belief;
    private float happiness;

    private float ressources;
    public Language Language;

    public bool hasSettlingLoc = false;
    public City city;

    public void Initialize()
    {
        Language = new Language();
    }
    
    public void SetPopulation(int population)
    {
        for (int i = 0; i < population; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void GetSettlingValues(Vector3Int vec)
    {
        food = TM.GetFood(vec);
        water = TM.GetWater(vec);
        safety = TM.GetSafety(vec);
        shelter = TM.GetShelter(vec);
        energy = TM.GetEnergy(vec);
    }

    private void CalcValues()
    {
        belief = (food + water + safety + shelter + energy) / 5; //TODO: include churches, actions by player etc. into this calculation
        happiness = (food + water + safety + shelter + energy) / 5 + (ressources / 5);
        ressources = 250; //TODO: adjust ressources dependant on tiles
    }

    private void CheckValues()
    {
        if (happiness < 100) SplitCivilisation();
    }

    private void SplitCivilisation()
    {
        //TODO: Split a civilisation
        Debug.Log("TODO: Split a civilisation");
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
}
