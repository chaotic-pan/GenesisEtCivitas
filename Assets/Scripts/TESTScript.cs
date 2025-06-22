using System;
using CityStuff;
using UnityEngine;

public class TESTScript : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private void BuildCity()
    {
        var go = Instantiate(prefab);
        var cell = TileManager.Instance.map.WorldToCell(transform.position);
        var cellCenterInWorld = TileManager.Instance.map.CellToWorld(cell);

        

        var cityPosition = AdjustCoordsForHeight(cellCenterInWorld);
        
        Debug.Log(cityPosition);
        
        go.transform.position = cityPosition;
        
        var city = go.GetComponent<City>();
        
        city.Initialize("TestCity");
        city.BuildWell();
    }
    
    private Vector3 AdjustCoordsForHeight(Vector3 coord)
    {
        var height = MapExtractor.Instance.GetHeightByWorldCoord(coord);
        return new Vector3(coord.x,height , coord.z);
    }

    public void OnBuildCity()
    {
        BuildCity();
    }
}
