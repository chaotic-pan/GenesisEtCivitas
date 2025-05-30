// MADE BY CIVITAS (technically Genesis but we don't talk about it)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terrain;
using UnityEngine;
using Utilities;

public class MapExtractor : MonoBehaviour
{
    [SerializeField] private string fileName = "file1.txt";
    [SerializeField] private MapDisplay mapDisplay;
    [SerializeField] private float mapHeightMultipier = 50f;
    [SerializeField] private TerrainType[] regions;
    //1913*1913 Punkte f�r die Gesamtmap
    private const int points = 1913;
    private const int totalPoints = 1913*1913;
    public AnimationCurve meshHeightCurve;

    void Start()
    {
        // Um gew�nschte Punkte mit Werten zu erhalten, muss von byte zu float[] zu float[,] transferiert werden
        var path = "./Assets/GenesisMap/"+ fileName;
        byte[] byteArray = File.ReadAllBytes(path);

        // �berall wo 0 ist, ist Wasser
        // Werte von 0-1 f�r Heightmap, 0-15 f�r alles andere, climate 0-255
        // Nur Heightmap ist in float, alle andere sind in bytes oder half bytes
        float[] floatArrayHeightMap = new float[totalPoints];
        byte[] fertilityFirmnessMap = new byte[totalPoints];
        byte[] oreVegetationMap = new byte[totalPoints];
        byte[] animalPopulationHostilityMap = new byte[totalPoints];
        byte[] climateMap = new byte[totalPoints];

        float[,] heightMap = new float[points, points];
        int[,] fertility = new int[points, points];
        int[,] firmness = new int[points, points];
        int[,] ore = new int[points, points];
        int[,] vegetation = new int[points, points];
        int[,] animalPopulation = new int[points, points];
        int[,] animalHostility = new int[points, points];
        int[,] climate = new int[points, points];

        // Bytedaten aus dem Bytearray werden in einzelne Bytearrays separiert
        // Startarray, Startnummer im Array, Zielarray, Startnummer im Zielarray, Gr��e
        // Byte array ist 4* so lang wie float array (4 bytes = 1 float)
        Buffer.BlockCopy(byteArray, 0, floatArrayHeightMap, 0, floatArrayHeightMap.Length*sizeof(float)); 
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 0), fertilityFirmnessMap, 0, fertilityFirmnessMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 1), oreVegetationMap, 0, oreVegetationMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 2), animalPopulationHostilityMap, 0, animalPopulationHostilityMap.Length);
        Buffer.BlockCopy(byteArray, totalPoints * (sizeof(float) + 3), climateMap, 0, climateMap.Length);

        // Separate bytes into 2D arrays for each value
        int i = 0;
        foreach (var coord in VectorUtils.GridCoordinates(points, points))
        {
            heightMap[coord.x, coord.y] = floatArrayHeightMap[i];
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
        var colorMap = WriteColorMap(heightMap, 8);
        mapDisplay.DrawMeshes(TerrainMeshGenerator.GenerateMesh(heightMap, mapHeightMultipier, meshHeightCurve, 8, 8), TextureGenerator.TextureFromColorMaps(colorMap, 240));
    }
    
    public Dictionary<Vector2, Color[]> WriteColorMap(float[,] noiseMap, int nHorizontalChunks)
    {
        var colorMaps = new Dictionary<Vector2, Color[]>();
        int ChunkSize = 240;
        foreach (var chunkCoord in VectorUtils.GridCoordinates(nHorizontalChunks, nHorizontalChunks))
        {
            //Reduce Offsets by 1 to have same values at seams
            var chunkXOffset = chunkCoord.x * (ChunkSize - 1);
            var chunkYOffset = chunkCoord.y * (ChunkSize - 1);

            var colorMap = new Color[ChunkSize * ChunkSize];
            foreach (var coord in VectorUtils.GridCoordinates(ChunkSize, ChunkSize))
            {
                var offsetX = coord.x + chunkXOffset;
                var offsetY = coord.y + chunkYOffset;
                var colorMapCoordinate = coord.y * ChunkSize + coord.x;

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

}
