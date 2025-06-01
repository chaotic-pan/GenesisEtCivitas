using UnityEngine;

public class CIVManager : MonoBehaviour
{
    // base scores food, water, safety, shelter and energy go from 0 to 500
    // combined scores belief and happiness are calculated
    // ressources are gathered and go up to 500

    [SerializeField] public string civilisationName;
    [SerializeField] private int food = 250;
    [SerializeField] private int water = 250;
    [SerializeField] private int safety = 250;
    [SerializeField] private int shelter = 250;
    [SerializeField] private int energy = 250;

    private int belief;
    private int happiness;

    private int ressources;

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
}
