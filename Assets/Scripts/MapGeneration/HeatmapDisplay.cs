using System;
using System.Collections.Generic;
using System.IO;
using Events;
using Terrain;
using UI;
using UnityEngine;
using Utilities;

namespace MapGeneration
{
    public class HeatmapDisplay : MonoBehaviour
    {
        [SerializeField] private MapDisplay mapDisplay;
        [SerializeField] private HeatmapGenerator heatmapGenerator;
        [SerializeField] private MapFileLocation SO_fileLoc;

        public readonly Dictionary<MapDisplay.MapOverlay, Dictionary<Vector2, Texture2D>> _maps = new();
        
        public MapDisplay.MapOverlay _currentMapOverlay;
        private TileManager _tileManager;
        private Dictionary<MapDisplay.MapOverlay, Func<TileData, float>> _overlayValueByTile;
        private Dictionary<MapDisplay.MapOverlay, Heatmap> _heatmapDict;
        private int _chunkSize;
        
        public static HeatmapDisplay Instance;
        
        private void Awake()
        {
            Instance = this;
            UIEvents.UIMap.OnOpenHeatmap += OnChangeMap;
            UIEvents.UIMap.OnUpdateHeatmapChunks += UpdateHeatMapChunks;

            if (SO_fileLoc.isBuild)
                GameEvents.Lifecycle.OnTileManagerFinishedInitializing += LoadTextures;
        }

        private void OnDestroy()
        {
            UIEvents.UIMap.OnOpenHeatmap -= OnChangeMap;
            UIEvents.UIMap.OnUpdateHeatmapChunks -= UpdateHeatMapChunks;
            GameEvents.Lifecycle.OnTileManagerFinishedInitializing -= LoadTextures;
        }

        private void Start()
        {
            heatmapGenerator.Initialize();
            
            _overlayValueByTile = heatmapGenerator.OverlayValueByTile;
            _chunkSize = HeatmapGenerator.ChunkSize;
            _heatmapDict = heatmapGenerator._heatmapDict;
            
            InitializeMaps();

            if (!SO_fileLoc.isBuild)
                LoadTexturesFromDrive();
        }
        
        

        private void LoadTextures()
        {
            var tilemapGen = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
            var heatmaps = heatmapGenerator.GenerateAllHeatmaps(tilemapGen);

            _maps[MapDisplay.MapOverlay.Terrain] = MapExtractor.Instance.GetTerrainTextures();
            
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    foreach (var overlay in _overlayValueByTile.Keys)
                    {
                        var chunk = new Vector2(x, y);
                        var tex = heatmaps[chunk][overlay];
                        _maps[overlay].Add(chunk, tex);
                    }
                }
            }
        }
        
        private void LoadTexturesFromDrive()
        {
            _maps[MapDisplay.MapOverlay.Terrain] = MapExtractor.Instance.GetTerrainTextures();
            
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    foreach (var overlay in _overlayValueByTile.Keys)
                    {
                        var bytes = File.ReadAllBytes($"{Application.dataPath}/2D/GeneratedHeatmaps/{overlay}_{x}_{y}.jpg");
                        var tex = new Texture2D(_chunkSize, _chunkSize, TextureFormat.ARGB32, false);
                        tex.LoadImage(bytes);
                        _maps[overlay].Add(new Vector2(x, y), tex);
                    }
                }
            }
        }

        private void InitializeMaps()
        {
            foreach (var mapOverlay in _overlayValueByTile.Keys)
            {
                _maps[mapOverlay] = new Dictionary<Vector2, Texture2D>();
            } 
        }
        
        private void UpdateHeatMapChunks(List<Vector2> chunkPos, MapDisplay.MapOverlay overlay)
        {
            foreach (var pos in chunkPos)
            {
                OnSingleHeatmapOnChunk(pos, overlay);
            }
            
            if (_currentMapOverlay == overlay)
                mapDisplay.ReplaceTexture(_maps[overlay]);
        }
        
        private void OnSingleHeatmapOnChunk(Vector2 chunk, MapDisplay.MapOverlay overlay)
        {
            var tileManager = TileManager.Instance;
            var worldPosition = new Vector3(0, 0, 0);
            var halfChunkSize = _chunkSize / 2;
            var colorMap = new Color[_chunkSize * _chunkSize];
            
            foreach (var coord in VectorUtils.GridCoordinates(_chunkSize, _chunkSize))
            {
                    worldPosition.x = (chunk.x * (_chunkSize - 1)) + (coord.x - halfChunkSize) + 1;
                    worldPosition.z = -(chunk.y * (_chunkSize - 1)) + (coord.y - halfChunkSize);
                    
                    var tile = tileManager.getTileDataByWorldCoords(worldPosition);
                    
                    var colorMapCoordinate = (_chunkSize - 1 - coord.y) * _chunkSize + coord.x;

                    if (tile == null)
                    {
                        var heatPointNull = HeatToColor(_heatmapDict[overlay].gradient,
                            _heatmapDict[overlay].min, _heatmapDict[overlay].min,
                            _heatmapDict[overlay].max);

                        colorMap[colorMapCoordinate] = heatPointNull;
                        continue;
                    }
                    
                    var tileValue = _overlayValueByTile[overlay](tile);
                        
                    var heatPoint = HeatToColor(_heatmapDict[overlay].gradient,
                        tileValue, _heatmapDict[overlay].min,
                        _heatmapDict[overlay].max);

                    colorMap[colorMapCoordinate] = heatPoint;
            }

            _maps[overlay][chunk] = TextureGenerator.TextureFromColorMap(colorMap, _chunkSize);
        }
        
        

        private void OnChangeMap(MapDisplay.MapOverlay mapOverlay)
        {
            mapDisplay.ReplaceTexture(_maps[mapOverlay]);
            _currentMapOverlay = mapOverlay;
        }

        public Color HeatToColor(Gradient gradient, float value, float min, float max)
        {
            var normalized = (value - min) / (max - min);
            return gradient.Evaluate(normalized);
        }
    }
}