using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private GameObject civilisationPrefab;
    [SerializeField] public List<GameObject> civilisations;
    [SerializeField] private int civilisationNumber = 4;

    private void Start()
    {

        for(int i=0; i< civilisationNumber; i++)
        {
            int random = Random.Range(0, TileManager.Instance.tiles.Count-1);
            Vector2Int tileLocation = TileManager.Instance.tiles.ElementAt(random).Key;
            Vector3 spawnLocation = TileManager.Instance.map.CellToWorld(new Vector3Int(tileLocation.x, tileLocation.y));
            var civ = Instantiate(civilisationPrefab, spawnLocation, Quaternion.identity);
            civilisations.Add(civ);
        }
    }
}
