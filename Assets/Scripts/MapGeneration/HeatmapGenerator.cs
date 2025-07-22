using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terrain;
using UnityEngine;
using Utilities;

namespace MapGeneration
{
    public class HeatmapGenerator : MonoBehaviour
    {
        [SerializeField] private List<Heatmap> heatmaps;
        [SerializeField] private GameObject tilemap;
        
        public const int ChunkSize = 240;
        private const int ChunkCount = 12;
        private const int HalfChunkSize = ChunkSize / 2;

        public readonly Dictionary<MapDisplay.MapOverlay, Func<TileData, float>> OverlayValueByTile = new()
        {
            {MapDisplay.MapOverlay.Travelcost, tile => tile.travelCost},
            {MapDisplay.MapOverlay.Fertility, tile => tile.landFertility},
            {MapDisplay.MapOverlay.AnimalHostility, tile => tile.animalHostility},
            {MapDisplay.MapOverlay.AnimalPopulation, tile => tile.animalPopulation},
            {MapDisplay.MapOverlay.Vegetation, tile => tile.vegetation},
            {MapDisplay.MapOverlay.Climate, tile => tile.climate},
            {MapDisplay.MapOverlay.WaterValue, tile => tile.waterValue},
            {MapDisplay.MapOverlay.Firmness, tile => tile.firmness},
            {MapDisplay.MapOverlay.Ore, tile => tile.ore},
        };
        
        public Dictionary<MapDisplay.MapOverlay, Heatmap> _heatmapDict = new();

        private readonly Dictionary<MapDisplay.MapOverlay, Color[]> _colorMapCacheDict = new();
        private MapDisplay.MapOverlay[] _overlayKeys;
        private string generatedHeatmapPath = $"{Application.dataPath}/2D/GeneratedHeatmaps";
        private float tilemapY;

        public void Initialize()
        {
            _heatmapDict = heatmaps.ToDictionary(h => h.overlay, h => h);
            _overlayKeys = OverlayValueByTile.Keys.ToArray();
            tilemapY = tilemap.transform.position.y;
            InitializeColorMapCacheDict();
        }
        
        public void GenerateAllHeatmapsAndWriteToDrive(TileManager tileManager)
        {
            Initialize();

            DeleteAllHeatmaps();
            
            for (int y = 0; y < ChunkCount; y++)
            {
                for (int x = 0; x < ChunkCount; x++)
                {
                    var chunkHeatmaps = CreateHeatmapsOnChunk(new Vector2(x, y), tileManager);
                    WriteHeatmapsToDrive(chunkHeatmaps, new Vector2(x, y));
                }
            }
        }

        public Dictionary<Vector2, Dictionary<MapDisplay.MapOverlay, Texture2D>> GenerateAllHeatmaps(TileManager tileManager)
        {
            Initialize();

            var heatmaps = new Dictionary<Vector2, Dictionary<MapDisplay.MapOverlay, Texture2D>>();
            
            for (var y = 2; y < ChunkCount - 2; y++)
            {
                for (var x = 2; x < ChunkCount - 2; x++)
                {
                    var chunk = new Vector2(x, y);
                    var chunkHeatmaps = CreateHeatmapsOnChunk(chunk, tileManager);
                    heatmaps.Add(chunk, chunkHeatmaps);
                }
            }
            
            return heatmaps;
        }
        

        private void InitializeColorMapCacheDict()
        {
            foreach (var overlay in OverlayValueByTile.Keys)
            {
                _colorMapCacheDict[overlay] = new Color[ChunkSize * ChunkSize];
            }
        }

        private Dictionary<MapDisplay.MapOverlay, Texture2D> CreateHeatmapsOnChunk(Vector2 chunk, TileManager tileManager)
        {
            var chunkOffsetX = chunk.x * (ChunkSize - 1) + 1;
            var chunkOffsetZ = -(chunk.y * (ChunkSize - 1));
            var worldPosition = new Vector3(0, tilemapY, 0);

            foreach (var overlay in _overlayKeys)
            {
                var colorMap = _colorMapCacheDict[overlay];
                Array.Clear(colorMap, 0, colorMap.Length);
            }

            foreach (var coord in VectorUtils.GridCoordinates(ChunkSize, ChunkSize))
            {
                    worldPosition.x = chunkOffsetX + coord.x - HalfChunkSize;
                    worldPosition.z = chunkOffsetZ + coord.y - HalfChunkSize;
                    
                    var tile = tileManager.getTileDataByWorldCoords(worldPosition);
                    var colorMapCoordinate = (ChunkSize - 1 - coord.y) * ChunkSize + coord.x;
                    
                    if (tile == null)
                    {
                        foreach (var overlay in _overlayKeys)
                        {
                            var heatmap = _heatmapDict[overlay];
                            var heatPointNull = HeatToColor(heatmap.gradient, heatmap.min, heatmap.min, heatmap.max);

                            _colorMapCacheDict[overlay][colorMapCoordinate] = heatPointNull;
                        }

                        continue;
                    }
                    
                    foreach (var overlay in _overlayKeys)
                    {
                        var tileValue = OverlayValueByTile[overlay](tile);
                        var heatmap = _heatmapDict[overlay];
                        
                        var heatPoint = HeatToColor(heatmap.gradient, tileValue, heatmap.min, heatmap.max);

                        _colorMapCacheDict[overlay][colorMapCoordinate] = heatPoint;
                    }
            }

            var chunkHeatmaps = new Dictionary<MapDisplay.MapOverlay, Texture2D>();
            
            foreach (var overlay in _overlayKeys)
            {
                var tileValue = _colorMapCacheDict[overlay];
                var tex = TextureGenerator.TextureFromColorMap(tileValue, ChunkSize);

                chunkHeatmaps[overlay] = tex;
            }

            return chunkHeatmaps;
        }

        private void WriteHeatmapsToDrive(Dictionary<MapDisplay.MapOverlay, Texture2D> chunkHeatmaps, Vector2 chunk)
        {
            foreach (var overlay in chunkHeatmaps.Keys)
            {
                byte[] bytes = ImageConversion.EncodeToJPG(chunkHeatmaps[overlay]);
                DestroyImmediate(chunkHeatmaps[overlay]);
                File.WriteAllBytes($"{generatedHeatmapPath}/{overlay}_{chunk.x}_{chunk.y}.jpg", bytes);
            }
        }
        
        private void DeleteAllHeatmaps()
        {
            try
            {
                if (!Directory.Exists(generatedHeatmapPath))
                {
                    Debug.LogWarning($"Folder does not exist: {generatedHeatmapPath}");
                    return;
                }
                
                string[] files = Directory.GetFiles(generatedHeatmapPath);
            
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting files: {e.Message}");
            }
        }

        private Color HeatToColor(Gradient gradient, float value, float min, float max)
        {
            var normalized = (value - min) / (max - min);
            return gradient.Evaluate(normalized);
        }
    }
}