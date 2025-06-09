using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject civilisationPrefab;
    [SerializeField] public List<GameObject> civilisations;
    [SerializeField] private int civilisationCount = 4;

    private void Start()
    {
        var cellBounds = TileManager.Instance.map.cellBounds;

        for(int i=0; i< civilisationCount; i++)
        {
            int randomX = Random.Range(cellBounds.min.x, cellBounds.max.x);
            int randomY = Random.Range(cellBounds.min.y, cellBounds.max.y);

            // re-roll location until you get a non-water tile
            while (TileManager.Instance.map.GetTile(new Vector3Int(randomX, randomY, 0)) == null 
                   /*|| TileManager.Instance.getTileDataByGridCoords(randomX, randomY).tileType.Contains("Water")*/)
            {
                randomX = Random.Range(cellBounds.min.x, cellBounds.max.x);
                randomY = Random.Range(cellBounds.min.y, cellBounds.max.y);
            }
            
            Vector3 spawnLocation = TileManager.Instance.map.CellToWorld(new Vector3Int(randomX,randomY));
            var civ = Instantiate(civilisationPrefab, spawnLocation, Quaternion.identity,transform);
            civilisations.Add(civ);
            civ.GetComponent<Civilization>().setPopulation(Random.Range(1,8));
        }
    }
}
