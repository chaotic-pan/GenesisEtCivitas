using System;
using CityStuff;
using Models;
using UnityEngine;

public class CityBuilder : MonoBehaviour
{
    public static CityBuilder Instance;
    
    [SerializeField] private GameObject cityPrefab;

    
    private void Awake()
    {
        Instance = this;
    }
    
    public City BuildCity(Vector3 worldPosition, NPCModel _npcModel, Civilization civ)
    {
        var cityInstance = Instantiate(cityPrefab);
        var cell = TileManager.Instance.map.WorldToCell(worldPosition);
        var cellCenterInWorld = TileManager.Instance.map.CellToWorld(cell);
        var cityPosition = AdjustCoordsForHeight(cellCenterInWorld);
        
        var city = cityInstance.GetComponent<City>();
        
        cityInstance.transform.position = cityPosition;
        city.Initialize(_npcModel, civ);
        // city.BuildWell();

        return city;
    }
    
    private Vector3 AdjustCoordsForHeight(Vector3 coord)
    {
        var height = MapExtractor.Instance.GetHeightByWorldCoord(coord);
        return new Vector3(coord.x,height , coord.z);
    }
}
