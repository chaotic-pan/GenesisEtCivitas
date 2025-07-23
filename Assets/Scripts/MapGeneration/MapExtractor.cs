// MADE BY CIVITAS (technically Genesis but we don't talk about it)

using MapGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MapGeneration.Maps;
using Terrain;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;

[ExecuteInEditMode]
public class MapExtractor : MonoBehaviour
{

    public static MapExtractor Instance;
    [SerializeField] private string fileName = "file1.txt";
    [SerializeField] private MapDisplay mapDisplay;
    [SerializeField] private HeatmapDisplay heatmapDisplay;
    [SerializeField] public float mapHeightMultiplier = 50f;
    [SerializeField] private TerrainType[] regions;
    //1913*1913 Punkte für die Gesamtmap
    public int chunkSize = 240;
    public int points = 2870;
    private int totalPoints;
    private int chunkCountRoot = 12;
    [SerializeField] private MapFileLocation SO_fileLoc;
    
    public float[,] heightMap;
    public float[,] travelcost;
    public int[,] fertility;
    public int[,] firmness;
    public int[,] ore;
    public int[,] vegetation;
    public int[,] animalPopulation;
    public int[,] animalHostility;
    public int[,] climate;
    public int[,] walkable;
    public int[,] water;
    public SoilType[,] soil;

    private void Awake()
    {
        
        Instance = this;
        if (SO_fileLoc.isBuild)
        {
            GenerateMap();
            return;
        }
        
        Initialize();
        
    }

    private void Initialize()
    {
        if (!fileName.Contains(".world")) fileName += ".world";

        // Um gewünschte Punkte mit Werten zu erhalten, muss von byte zu float[] zu float[,] transferiert werden
        var path = "./Assets/GenesisMap/" + fileName;
        if (SO_fileLoc.isBuild && SO_fileLoc.MapLocation != null) path = SO_fileLoc.MapLocation;
        
        totalPoints = points*points;
        
        heightMap = new float[points, points];
        travelcost = new float[points, points];
        fertility = new int[points, points];
        firmness = new int[points, points];
        ore = new int[points, points];
        vegetation = new int[points, points];
        animalPopulation = new int[points, points];
        animalHostility = new int[points, points];
        climate = new int[points, points];
        walkable = new int[points, points];
        water = new int[points, points];
        soil = new SoilType[points, points];
        
        
        byte[] byteArray = File.ReadAllBytes(path);
        
        // Werte von 0-1 für Heightmap, 0-15 für alles andere, climate und water 0-255
        // Nur Heightmap ist in float, alle andere sind in bytes oder half bytes
        float[] floatArrayHeightMap = new float[totalPoints];
        byte[] fertilityFirmnessMap = new byte[totalPoints];
        byte[] oreVegetationMap = new byte[totalPoints];
        byte[] animalPopulationHostilityMap = new byte[totalPoints];
        byte[] climateMap = new byte[totalPoints];
        byte[] walkableMap = new byte[totalPoints];
        byte[] waterMap = new byte[totalPoints];
        // rain 0-255

        // Bytedaten aus dem Bytearray werden in einzelne Bytearrays separiert
        // Startarray, Startnummer im Array, Zielarray, Startnummer im Zielarray, Größe
        // Byte array ist 4* so lang wie float array (4 bytes = 1 float)
        Buffer.BlockCopy(byteArray, 0,
            floatArrayHeightMap, 0, floatArrayHeightMap.Length * sizeof(float));
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 0),
            fertilityFirmnessMap, 0, fertilityFirmnessMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 1),
            oreVegetationMap, 0, oreVegetationMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 2),
            animalPopulationHostilityMap, 0, animalPopulationHostilityMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 3),
            climateMap, 0, climateMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 4), 
            waterMap, 0, waterMap.Length);

        // Separate bytes into 2D arrays for each value
        int i = 0;
        foreach (var coord in VectorUtils.GridCoordinates(points, points))
        {
            heightMap[coord.x, coord.y] = Math.Max(floatArrayHeightMap[i], 0);
            ore[coord.x, coord.y] = (int)oreVegetationMap[i] & 0xF;
            vegetation[coord.x, coord.y] = (int)(oreVegetationMap[i] >> 4) & 0xF;
            animalPopulation[coord.x, coord.y] = (int)animalPopulationHostilityMap[i] & 0xF;
            animalHostility[coord.x, coord.y] = (int)(animalPopulationHostilityMap[i] >> 4) & 0xF;
            climate[coord.x, coord.y] = (int)climateMap[i];
            
            SoilType currentSoil = GetSoilTypeForInt((int)fertilityFirmnessMap[i]);
            fertility[coord.x, coord.y] = GetFertility(currentSoil);
            firmness[coord.x, coord.y] = GetFirmness(currentSoil);
            soil[coord.x, coord.y] = currentSoil;
            walkable[coord.x, coord.y] = currentSoil is SoilType.Seafloor or SoilType.Riverbed ? 0 : 1;
            
            water[coord.x, coord.y] = walkable[coord.x, coord.y]==1? (int)waterMap[i] : 0;
            
            i++;
        }
        
        CalculateTravelCost();
    }

    public void GenerateMap()
    {
        // Delete previous map
        foreach(var chunk in GameObject.FindGameObjectsWithTag("MapChunk"))
        {
            DestroyImmediate(chunk);
        }

        Initialize();
        
        // Map wird generiert
        var colorMap = WriteColorMap(heightMap, chunkCountRoot, walkable);
        var textures = TextureGenerator.TextureFromColorMaps(colorMap, chunkSize);

        mapDisplay.DrawMeshes(
            TerrainMeshGenerator.GenerateMesh(
                heightMap, mapHeightMultiplier, chunkCountRoot, points),
            textures);
    }

    private void CalculateTravelCost()
    {
       foreach (var coord in VectorUtils.GridCoordinates(points, points))
       {
           travelcost[coord.x, coord.y] = walkable[coord.x,coord.y] == 0 ? 20 :
               heightMap[coord.x, coord.y]*mapHeightMultiplier;
       }

    }
    public Dictionary<Vector2, Texture2D> GetTerrainTextures()
    {
        var colorMap = WriteColorMap(heightMap, chunkCountRoot, walkable);
        var textures = TextureGenerator.TextureFromColorMaps(colorMap, chunkSize);
        
        return textures;
    }

    private void Start()
    {
        if (TileManager.Instance != null) TileManager.Instance.InitializeTileData(Instance);
    }

    public Dictionary<Vector2, Color[]> WriteColorMap(float[,] noiseMap, int nHorizontalChunks, int[,] walkable)
    {
        var colorMaps = new Dictionary<Vector2, Color[]>();
        foreach (var chunkCoord in VectorUtils.GridCoordinates(nHorizontalChunks, nHorizontalChunks))
        {
            //Reduce Offsets by 1 to have same values at seams
            var chunkXOffset = chunkCoord.x * (chunkSize - 1);
            var chunkYOffset = chunkCoord.y * (chunkSize - 1);

            var colorMap = new Color[chunkSize * chunkSize];
            foreach (var coord in VectorUtils.GridCoordinates(chunkSize, chunkSize))
            {
                var offsetX = coord.x + chunkXOffset;
                var offsetY = coord.y + chunkYOffset;
                var colorMapCoordinate = coord.y * chunkSize + coord.x;

                var currentHeight = noiseMap[offsetX, offsetY];
                
                if(walkable[offsetX, offsetY] == 0)
                {
                    colorMap[colorMapCoordinate] = regions[0].color;
                    continue;
                }
                if (!regions.Any(region => region.height > currentHeight))
                    continue;
                var matchingRegion = regions.First(region => region.height > currentHeight);

                colorMap[colorMapCoordinate] = matchingRegion.color * 1f;

            }

            colorMaps.Add(new Vector2(chunkCoord.x, chunkCoord.y), colorMap);
        }

        return colorMaps;
    }

    private SoilType GetSoilTypeForInt(int i)
    {
        return i switch
        {
            0 => SoilType.BlackEarth,
            1 => SoilType.BrownEarth,
            2 => SoilType.Desert,
            3 => SoilType.HalfDesert,
            4 => SoilType.Ice,
            5 => SoilType.Jungle,
            6 => SoilType.Podzol,
            7 => SoilType.Seafloor,
            8 => SoilType.Swamp,
            9 => SoilType.Riverbed,
            10 => SoilType.Rock,
            11 => SoilType.TerraRossa,
            _ => SoilType.Wetland
        };
    } 
    
    public static int GetFertility(SoilType soil)
    {
        return soil switch
        {
            SoilType.BlackEarth => 15,
            SoilType.BrownEarth => 13,
            SoilType.Desert => 2,
            SoilType.HalfDesert => 4,
            SoilType.Ice => 4,
            SoilType.Jungle => 8,
            SoilType.Podzol => 5,
            SoilType.Seafloor => 0,
            SoilType.Swamp => 11,
            SoilType.Riverbed => 10,
            SoilType.Rock => 1,
            SoilType.TerraRossa => 7,
            SoilType.Wetland => 12,
            _ => 8,
        };
    }
    
    public static int GetFirmness(SoilType soil)
    {
        return soil switch
        {
            SoilType.BlackEarth => 10,
            SoilType.BrownEarth => 12,
            SoilType.Desert => 7,
            SoilType.HalfDesert => 8,
            SoilType.Ice => 4,
            SoilType.Jungle => 7,
            SoilType.Podzol => 9,
            SoilType.Seafloor => 0,
            SoilType.Swamp => 2,
            SoilType.Riverbed => 6,
            SoilType.Rock => 15,
            SoilType.TerraRossa => 13,
            SoilType.Wetland => 3,
            _ => 8,
        };
    }

    public float GetHeightByWorldCoord(Vector3 coord)
    {
        var p = CoordsToPoints(coord);
        return heightMap[p.x, p.y] * mapHeightMultiplier;
    }
    

    public Vector2Int CoordsToPoints(Vector3 coord)
    {
        return new Vector2Int((int)coord.x + chunkSize / 2, -(int)coord.z + chunkSize / 2);
    }
    
    public Vector3 AdjustCoordsForHeight(Vector3 coord)
    {
        var height = GetHeightByWorldCoord(coord);
        return new Vector3(coord.x,height , coord.z);
    }

    public bool IsWalkable(Vector3 coord)
    {
        var p = CoordsToPoints(coord);
        return walkable[p.x, p.y] != 0;
    }

    
}
 


