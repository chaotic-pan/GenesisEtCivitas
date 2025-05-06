using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    
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
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = map.WorldToCell(mousePos);
            
            
            var clickedTileData = getTileDataByCoords(mousePos);

            string tileType = clickedTileData.tileType;
            float travelCost = clickedTileData.travelCost;
            float waterValue = clickedTileData.waterValue;
            float landFertility = clickedTileData.landFertility;
            
            print(clickedTileData != null ? "At position " + gridPos + " there is a " + tileType 
                + " Tile with: \nTravelCost=" + travelCost + ", WaterValue=" + waterValue + ", LandFertility=" + landFertility 
                                      : "At position " + gridPos + " there is no tile ");
        }
    }

    public TileData getTileDataByCoords(Vector2 coordinates)
    {
        Vector3Int gridPos = map.WorldToCell(coordinates);
        return dataFromTiles[map.GetTile(gridPos)];
    }
}
