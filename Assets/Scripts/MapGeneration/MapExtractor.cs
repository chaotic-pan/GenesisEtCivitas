// MADE BY CIVITAS (technically Genesis but we don't talk about it)

using MapGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public float waterheight = 0.21f;
    [SerializeField] private TerrainType[] regions;
    //1913*1913 Punkte für die Gesamtmap
    [SerializeField] private int points = 1914;
    private int totalPoints;
    private int chunkSize = 240;
    private int chunkCountRoot = 8;
    public AnimationCurve meshHeightCurve;
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

    private void Awake()
    {
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
        
        
        Instance = this;
        if (SO_fileLoc.isBuild)
        {
            GenerateMap();
            return;
        }

        // Um gewünschte Punkte mit Werten zu erhalten, muss von byte zu float[] zu float[,] transferiert werden
        var path = "./Assets/GenesisMap/" + fileName;
        byte[] byteArray = File.ReadAllBytes(path);

        // überall wo 0 ist, ist Wasser
        // Werte von 0-1 für Heightmap, 0-15 für alles andere, climate 0-255
        // Nur Heightmap ist in float, alle andere sind in bytes oder half bytes
        float[] floatArrayHeightMap = new float[totalPoints];
        byte[] fertilityFirmnessMap = new byte[totalPoints];
        byte[] oreVegetationMap = new byte[totalPoints];
        byte[] animalPopulationHostilityMap = new byte[totalPoints];
        byte[] climateMap = new byte[totalPoints];

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

        // Separate bytes into 2D arrays for each value
        int i = 0;
        foreach (var coord in VectorUtils.GridCoordinates(points, points))
        {
            heightMap[coord.x, coord.y] = Math.Max(floatArrayHeightMap[i], 0);
            fertility[coord.x, coord.y] = (int)fertilityFirmnessMap[i] & 0xF;
            firmness[coord.x, coord.y] = (int)(fertilityFirmnessMap[i] >> 4) & 0xF;
            ore[coord.x, coord.y] = (int)oreVegetationMap[i] & 0xF;
            vegetation[coord.x, coord.y] = (int)(oreVegetationMap[i] >> 4) & 0xF;
            animalPopulation[coord.x, coord.y] = (int)animalPopulationHostilityMap[i] & 0xF;
            animalHostility[coord.x, coord.y] = (int)(animalPopulationHostilityMap[i] >> 4) & 0xF;
            climate[coord.x, coord.y] = (int)climateMap[i];

            i++;
        }
    }

    public void GenerateMap()
    {
        // Delete previous map
        foreach(var chunk in GameObject.FindGameObjectsWithTag("MapChunk"))
        {
            DestroyImmediate(chunk);
        }
        
        // Um gewünschte Punkte mit Werten zu erhalten, muss von byte zu float[] zu float[,] transferiert werden
        var path = "./Assets/GenesisMap/" + fileName;
        if (SO_fileLoc.isBuild && SO_fileLoc.MapLocation != null) path = SO_fileLoc.MapLocation;
        byte[] byteArray = File.ReadAllBytes(path);
        
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
        
        // überall wo 0 ist, ist Wasser
        // Werte von 0-1 für Heightmap, 0-15 für alles andere, climate 0-255
        // Nur Heightmap ist in float, alle andere sind in bytes oder half bytes
        float[] floatArrayHeightMap = new float[totalPoints];
        byte[] fertilityFirmnessMap = new byte[totalPoints];
        byte[] oreVegetationMap = new byte[totalPoints];
        byte[] animalPopulationHostilityMap = new byte[totalPoints];
        byte[] climateMap = new byte[totalPoints];

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

        // Separate bytes into 2D arrays for each value
        int i = 0;
        foreach (var coord in VectorUtils.GridCoordinates(points, points))
        {
            heightMap[coord.x, coord.y] = Math.Max(floatArrayHeightMap[i], 0);
            fertility[coord.x, coord.y] = (int)fertilityFirmnessMap[i] & 0xF;
            firmness[coord.x, coord.y] = (int)(fertilityFirmnessMap[i] >> 4) & 0xF;
            ore[coord.x, coord.y] = (int)oreVegetationMap[i] & 0xF;
            vegetation[coord.x, coord.y] = (int)(oreVegetationMap[i] >> 4) & 0xF;
            animalPopulation[coord.x, coord.y] = (int)animalPopulationHostilityMap[i] & 0xF;
            animalHostility[coord.x, coord.y] = (int)(animalPopulationHostilityMap[i] >> 4) & 0xF;
            climate[coord.x, coord.y] = (int)climateMap[i];

            i++;
        }

        // Map wird generiert
        var colorMap = WriteColorMap(heightMap, chunkCountRoot);
        var textures = TextureGenerator.TextureFromColorMaps(colorMap, chunkSize);

        mapDisplay.DrawMeshes(
            TerrainMeshGenerator.GenerateMesh(
                heightMap, mapHeightMultiplier, meshHeightCurve, chunkCountRoot, chunkCountRoot),
            textures);
    }

    public Dictionary<Vector2, Texture2D> GetTerrainTextures()
    {
        var colorMap = WriteColorMap(heightMap, chunkCountRoot);
        var textures = TextureGenerator.TextureFromColorMaps(colorMap, chunkSize);
        
        return textures;
    }

    private void Start()
    {
        TileManager.Instance.InitializeTileData(Instance);
    }

    public Dictionary<Vector2, Color[]> WriteColorMap(float[,] noiseMap, int nHorizontalChunks)
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
                
                if (!regions.Any(region => region.height > currentHeight))
                    continue;
                var matchingRegion = regions.First(region => region.height > currentHeight);

                colorMap[colorMapCoordinate] = matchingRegion.color * 1f;

            }

            colorMaps.Add(new Vector2(chunkCoord.x, chunkCoord.y), colorMap);
        }

        return colorMaps;
    }

    public float GetHeightByWorldCoord(Vector3 coord)
    {
        var p = CoordsToPoints(coord);
        return meshHeightCurve.Evaluate(heightMap[p.x, p.y]) * mapHeightMultiplier;
    }
    

    public Vector2Int CoordsToPoints(Vector3 coord)
    {
        return new Vector2Int((int)coord.x + chunkSize / 2, -(int)coord.z + chunkSize / 2);
    }

}
