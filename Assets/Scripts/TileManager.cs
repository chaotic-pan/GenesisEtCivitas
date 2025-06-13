using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    [SerializeField] public Tilemap map;
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;
    
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
    }

    public TileData getTileDataByWorldCoords(float x, float y, float z)
    {
        return getTileDataByWorldCoords(new Vector3(x, y, z));
    }
    public TileData getTileDataByWorldCoords(Vector3 coordinates)
    {
        var gridPos = GetTileGridPositionByWorldCoords(coordinates);

        var tile = map.GetTile(gridPos);
        return tile != null ? dataFromTiles[tile] : null;
    }

    public Vector3Int GetTileGridPositionByWorldCoords(Vector3 coordinates)
        => map.WorldToCell(coordinates);
    
    public TileData getTileDataByGridCoords(int gridX, int gridY)
    {
        return getTileDataByGridCoords(new Vector3Int(gridX,gridY));
    }
    public TileData getTileDataByGridCoords(Vector3Int gridPos)
    {

        var tile = map.GetTile(gridPos);
        return tile != null ? dataFromTiles[tile] : null;
    }
   
    public void printTileData(int gridX, int gridY) 
    {
        printTileData(new Vector3Int(gridX, gridY));
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
    
    
    /// <summary>
    /// qrs to xy(0)
    /// </summary>
    /// <param name="cubePos"></param>
    /// <returns></returns>
    public Vector3Int CubeToGrid(Vector3Int cubePos)
    { 
        var x = cubePos.x + (cubePos.y - (cubePos.y & 1)) / 2;
        var y = cubePos.y;
        return new Vector3Int(x, y);
    }
    /// <summary>
    /// qrs to xy(0)
    /// </summary>
    /// <param name="q"></param>
    /// <param name="r"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    public Vector3Int CubeToGrid(int q, int r, int s)
    {
        return CubeToGrid(new Vector3Int(q,r,s));
    }
    
    /// <summary>
    /// xy(0) to qrs
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public Vector3Int GridToCube(Vector3Int gridPos)
    { 
        var q = gridPos.x - (gridPos.y - (gridPos.y & 1)) / 2;
        var r = gridPos.y;
        return new Vector3Int(q, r, -q - r);
    }
    /// <summary>
    /// xy(0) to qrs
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3Int GridToCube(int x, int y)
    {
        return GridToCube(new Vector3Int(x, y));
    }

    public List<Vector3Int> getNeighbors(Vector3Int gridPos)
    {
        var cubePos = GridToCube(gridPos);
        return new List<Vector3Int>
        {
            CubeToGrid(cubePos.x + 1, cubePos.y, cubePos.z - 1),
            CubeToGrid(cubePos.x + 1, cubePos.y - 1, cubePos.z),
            CubeToGrid(cubePos.x, cubePos.y-1, cubePos.z+1),
            CubeToGrid(cubePos.x-1, cubePos.y, cubePos.z+1),
            CubeToGrid(cubePos.x-1, cubePos.y+1, cubePos.z),
            CubeToGrid(cubePos.x, cubePos.y+1, cubePos.z-1)
        };
    }

    public List<Vector3Int> GetSpecificRange(Vector3Int gridPos, int radius)
    {
        var newRange = new List<Vector3Int>();
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = Math.Max(-radius, -q - radius); r <= Math.Min(radius, -q + radius); r++)
            {
                var s = -q - r;
                var cubePos = GridToCube(gridPos);
                newRange.Add(CubeToGrid(cubePos.x + q, cubePos.y + r, gridPos.z + s));
            }
        }

        return newRange;
    }
}
