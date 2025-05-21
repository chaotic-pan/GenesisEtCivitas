using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] public Tilemap map;
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     var mousePos = Input.mousePosition;
        //     var worldPos = getWorldPositionOnPlane(mousePos);
        //
        //     
        //     var clickedTileData = getTileDataByCoords(worldPos);
        //
        //     Vector3Int gridPos = map.WorldToCell(worldPos);
        //     
        //     if (clickedTileData == null)
        //     {
        //         print("At position " + gridPos + " there is no tile ");
        //         return;
        //     }
        //     
        //     string tileType = clickedTileData.tileType;
        //     float travelCost = clickedTileData.travelCost;
        //     float waterValue = clickedTileData.waterValue;
        //     float landFertility = clickedTileData.landFertility;
        //     
        //     print("At position " + gridPos + " there is a " + tileType 
        //         + " Tile with: \nTravelCost=" + travelCost + ", WaterValue=" + waterValue + ", LandFertility=" + landFertility);
        // }
    }

    public TileData getTileDataByCoords(Vector3 coordinates)
    {
        //print(coordinates);
        Vector3Int gridPos = map.WorldToCell(coordinates);
        
        // print(gridPos);
        
        var tile = map.GetTile(gridPos);
        return tile != null ? dataFromTiles[tile] : null;
    }

    public void printTileData(Vector3Int gridPos)
    {
        var tile = map.GetTile(gridPos);
        var clickedTileData = tile != null ? dataFromTiles[tile] : null;
        
        if (clickedTileData == null)
        {
            print("At position " + gridPos + " there is no tile ");
            return;
        }
                
        string tileType = clickedTileData.tileType;
        float travelCost = clickedTileData.travelCost;
        float waterValue = clickedTileData.waterValue;
        float landFertility = clickedTileData.landFertility;
                
        print("At position " + gridPos + " there is a " + tileType 
              + " Tile with: \nTravelCost=" + travelCost + ", WaterValue=" + waterValue + ", LandFertility=" + landFertility);
    }

    public Vector3 getWorldPositionOnPlane(Vector3 screenPosition) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0.78f, 0));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
