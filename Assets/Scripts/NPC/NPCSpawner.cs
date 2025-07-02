using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject civilisationPrefab;
    [SerializeField] public List<GameObject> civilisations;
    [SerializeField] private int civilisationCount = 4;
    private TileManager TM;

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
        }
    }
}
