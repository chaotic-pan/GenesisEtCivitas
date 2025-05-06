using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        // string[] files = Directory.GetFiles(path, "*.asset", SearchOption.TopDirectoryOnly);
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

            TileBase clickedTile = map.GetTile(gridPos);

            float travelCost = dataFromTiles[clickedTile].travelCost;
            float waterValue = dataFromTiles[clickedTile].waterValue;
            float landFertility = dataFromTiles[clickedTile].landFertility;
            
            print(clickedTile != null ? "At position " + gridPos + " there is a " + clickedTile 
                + "with TravelCost=" + travelCost + ", WaterValue=" + waterValue + ", LandFertility=" + landFertility 
                                      : "At position " + gridPos + " there is no tile ");
        }
    }
}
//