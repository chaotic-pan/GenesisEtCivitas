using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    [SerializeField] public Tilemap map;
    private Dictionary<Vector3Int, TileData> dataFromTiles = new();
    public List<Vector3Int> spawnLocations = new();
    
    private void Awake()
    {
        Instance = this;
    }

    public void InitializeTileData(MapExtractor ME)
    {
        for (int x = map.cellBounds.min.x; x < map.cellBounds.max.x; x++)
        {
            for (int y = map.cellBounds.min.y; y < map.cellBounds.max.y; y++)
            {
                var gridPos = new Vector3Int(x, y, 0);
                TileBase tile = map.GetTile(gridPos);
                if (tile != null)
                {
                    var p = ME.CoordsToPoints( map.CellToWorld(new Vector3Int(x, y, 0)));
                    var height = ME.meshHeightCurve.Evaluate(ME.heightMap[p.x, p.y]) * ME.mapHeightMultiplier;
                    TileData tileData = new TileData(
                        ME.travelcost[p.x,p.y],
                        ME.fertility[p.x,p.y],
                        ME.firmness[p.x,p.y],
                        ME.ore[p.x,p.y],
                        ME.vegetation[p.x,p.y],
                        ME.animalPopulation[p.x, p.y],
                        ME.animalHostility[p.x, p.y],
                        ME.climate[p.x,p.y],
                        height <= 0.1 ? 30 : 1,  //water value
                        height
                        ); 
                    dataFromTiles.TryAdd(gridPos, tileData);
                    if (tileData.height > 0.1)
                    {
                        spawnLocations.Add(gridPos);
                    }
                }
            }
        }
    }


    /* Food: fertility, animalPopulation
     * Water: heightMap
     * Safety: animalHostility, heightMap
     * Shelter: firmness, ore, vegetation
     * Energy: climate */


    public float GetFood(Vector3Int coords)
    {
        return dataFromTiles.ContainsKey(coords) ? 
            (dataFromTiles[coords].landFertility + dataFromTiles[coords].animalPopulation)/2 
            : -1;
    }
    public float GetWater(Vector3Int coords)
    {
        //TODO
        return 1;
    }
    public float GetSafety(Vector3Int coords)
    {
        if (dataFromTiles.ContainsKey(coords))
        {
            float height = MapExtractor.Instance.meshHeightCurve.Evaluate(dataFromTiles[coords].height) * MapExtractor.Instance.mapHeightMultiplier;
            height = 0.1f <= height && height <= 0.7f ? 15f : 0;
            return (height + dataFromTiles[coords].animalHostility) / 2;
        }

        return -1;
    }
    public float GetShelter(Vector3Int coords)
    {
        return   dataFromTiles.ContainsKey(coords) ? 
            (dataFromTiles[coords].firmness + dataFromTiles[coords].ore + dataFromTiles[coords].vegetation) / 3 
            : -1;
    }
    public float GetEnergy(Vector3Int coords)
    {
        //TODO change it to a way where the value is better the closer it is to smth like 20 degree celsius
        return  dataFromTiles.ContainsKey(coords) ? 
            ((dataFromTiles[coords].climate + 1) / 16) - 1 
            : -1;
    }

    public TileData getTileDataByWorldCoords(float x, float y, float z)
    {
        return getTileDataByWorldCoords(new Vector3(x, y, z));
    }
    public TileData getTileDataByWorldCoords(Vector3 coordinates)
    {
        var gridPos = map.WorldToCell(coordinates);
        
        return getTileDataByGridCoords(gridPos);
    }

    public TileData getTileDataByGridCoords(int gridX, int gridY)
    {
        return getTileDataByGridCoords(new Vector3Int(gridX,gridY));
    }
    public TileData getTileDataByGridCoords(Vector3Int gridPos)
    {
        return dataFromTiles.ContainsKey(gridPos) ? dataFromTiles[gridPos] : null;
    }

    public List<Vector2> getWorldPositionOfTile(Vector3Int gridPos)
    {
        var cellIndexX = 14;
        var cellIndexY = 16;

        var chunkOffsetX = 4;
        var chunkOffsetY = -3;
        
        var offsetX = 3;
        var offsetY = -2;
        
        var cellPosWithOffset = new Vector2(gridPos.x + offsetX, gridPos.y + offsetY);
        var flooredCellPos = new Vector2((int)Math.Floor(cellPosWithOffset.x / cellIndexX), (int)Math.Floor(cellPosWithOffset.y / cellIndexY));

        var hitChunk = new List<Vector2>();
        
        hitChunk.Add(new Vector2(flooredCellPos.x + chunkOffsetX, Math.Abs(flooredCellPos.y + chunkOffsetY)));

        if (cellPosWithOffset.x % cellIndexX == 0)
            hitChunk.Add(new Vector2(flooredCellPos.x - 1 + chunkOffsetX,Math.Abs(flooredCellPos.y + chunkOffsetY)));
        
        if (cellPosWithOffset.y % cellIndexY == 0)
            hitChunk.Add(new Vector2(flooredCellPos.x + chunkOffsetX, Math.Abs(flooredCellPos.y - 1 +chunkOffsetY)));
        
        if (cellPosWithOffset.x % cellIndexX == 0 && cellPosWithOffset.y % cellIndexY == 0)
            hitChunk.Add(new Vector2(flooredCellPos.x - 1 + chunkOffsetX, Math.Abs(flooredCellPos.y - 1 + chunkOffsetY)));
        
        return hitChunk;
    }
    
    public Vector2 GetChunkForTile(Vector3Int tilePos)
    {
        Vector3 worldPos = map.CellToWorld(tilePos);
    
        int chunkX = Mathf.FloorToInt((worldPos.x + 120) / 239f);
        int chunkY = Mathf.FloorToInt((-worldPos.z + 120) / 239f);
    
        return new Vector2(chunkX, chunkY);
    }
   
    public void printTileData(int gridX, int gridY) 
    {
        printTileData(new Vector3Int(gridX, gridY));
    }
    public void printTileData(Vector3Int gridPos)
    {
        if (!dataFromTiles.ContainsKey(gridPos))
        {
            print("At position " + gridPos + " there is no tile ");
            return;
        }
        
        var clickedTileData = dataFromTiles[gridPos]; 
        
        float travelCost = clickedTileData.travelCost;
        float waterValue = clickedTileData.waterValue;
        float landFertility = clickedTileData.landFertility;
                
        print("At position " + gridPos + " there is a Tile with: \nTravelCost=" 
              + travelCost + ", WaterValue=" + waterValue + ", LandFertility=" + landFertility);
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
    
    public Bounds GetTileBounds(Vector3Int gridPos)
    {
        Vector3 center = map.GetCellCenterWorld(gridPos);
        Vector3 size = map.cellSize;
        return new Bounds(center, size);
    }

    public List<Vector3Int> GetSpecificRange(Vector3Int gridPos, int radius)
    {
        if (radius < 0) return new List<Vector3Int> { gridPos };
    
        var newRange = new List<Vector3Int>();
        var centerCube = GridToCube(gridPos);
    
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = Math.Max(-radius, -dx - radius); dy <= Math.Min(radius, -dx + radius); dy++)
            {
                int dz = -dx - dy;
                Vector3Int cubePos = new Vector3Int(
                    centerCube.x + dx,
                    centerCube.y + dy,
                    centerCube.z + dz
                );
                newRange.Add(CubeToGrid(cubePos));
            }
        }
        return newRange;
    }
    
    public List<Vector3Int> GetFullRange()
    {
        return dataFromTiles.Keys.ToList();
    }
}
