using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile
{
    public Tile(int x, int y)
    {
        pos = new Vector2Int(x, y);
    }
    
    public Vector2Int pos;
    public Dictionary<Tile, float> neighbors = new Dictionary<Tile, float>();
}

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    [SerializeField] public Tilemap map;
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    private void Awake()
    {
        Instance = this;

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
                addNeighborRelation(x,y,-1,0,tile);
                
                // (LEFT DOWN)
                    // -1 -1 on even y 
                    //  0 -1 on odd y
                var q = y % 2 == 0 ? -1 : 0;
                addNeighborRelation(x,y,q,-1,tile);
                
                // (RIGHT DOWN) 
                    // 0 -1 on even y 
                    // 1 -1 on odd y
                q = y % 2 == 0 ? 0 : 1;
                addNeighborRelation(x,y,q,-1,tile);
               
            }
        }
    }

    private void addNeighborRelation(int x, int y, int q, int w, Tile tile)
    {
        if (tiles.ContainsKey(new Vector2Int(x + q, y + w)))
        {
            var neighbor = tiles[new Vector2Int(x + q, y + w)];
            neighbor.neighbors.Add(tile, dataFromTiles[map.GetTile(new Vector3Int(tile.pos.x, tile.pos.y, 0))].travelCost);
            tile.neighbors.Add(neighbor, dataFromTiles[map.GetTile(new Vector3Int(neighbor.pos.x, neighbor.pos.y, 0))].travelCost);
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
    
    public TileData getTileDataByWorldCoords(Vector3 coordinates)
    {
        Vector3Int gridPos = map.WorldToCell(coordinates);

        var tile = map.GetTile(gridPos);
        return tile != null ? dataFromTiles[tile] : null;
    }
    
    public TileData getTileDataByGridCoords(Vector2Int gridPos)
    {

        var tile = map.GetTile(new Vector3Int(gridPos.x, gridPos.y, 0));
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
