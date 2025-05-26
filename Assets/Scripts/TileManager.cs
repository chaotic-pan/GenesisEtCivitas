using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile
{
    public Tile(int x, int y)
    {
        positions = new Vector2Int(x, y);
    }
    
    public Vector2Int positions;
    public Dictionary<Tile, float> neighbors = new Dictionary<Tile, float>();
}

public class TileManager : MonoBehaviour
{
    [SerializeField] public Tilemap map;
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
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

        for (int y = map.cellBounds.min.y; y < map.cellBounds.max.y; y++)
        {
            for (int x = map.cellBounds.min.x; x < map.cellBounds.max.x; x++)
            {
                
                if (map.GetTile(new Vector3Int(x, y, 0)) == null) continue;
                var tile = new Tile(x, y);
                tiles.Add(new Vector2Int(x,y), tile);
                
                // -1 0 (LEFT)
                if (tiles.ContainsKey(new Vector2Int(x - 1, y)))
                {
                    var neighbor = tiles[new Vector2Int(x - 1, y)];
                    neighbor.neighbors.Add(tile, 1);
                    tile.neighbors.Add(neighbor, 1);
                }
                
                // (LEFT DOWN)
                    // -1 -1 on even y 
                    //  0 -1 on odd y
                var q = y % 2 == 0 ? -1 : 0;
                if (tiles.ContainsKey(new Vector2Int(x + q, y - 1)))
                {
                    var neighbor = tiles[new Vector2Int(x + q, y - 1)];
                    neighbor.neighbors.Add(tile, 1);
                    tile.neighbors.Add(neighbor, 1);
                }
                
                // (RIGHT DOWN) 
                    // 0 -1 on even y 
                    // 1 -1 on odd y
                q = y % 2 == 0 ? 0 : 1;
                if (tiles.ContainsKey(new Vector2Int(x + q, y - 1)))
                {
                    var neighbor = tiles[new Vector2Int(x + q, y - 1)];
                    neighbor.neighbors.Add(tile, 1);
                    tile.neighbors.Add(neighbor, 1);
                }
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

    public Tile getTileByGridCoords(Vector2Int coordinates)
    {
        return tiles[coordinates];
    }
    
    public TileData getTileDataByWorldCoords(Vector2Int coordinates)
    {
        Vector3Int gridPos = map.WorldToCell(new Vector3(coordinates.x, coordinates.y, 0));

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
}
