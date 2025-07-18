using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Terrain;
using UI;
using Unity.Jobs;
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
        private Dictionary<Vector2, Dictionary<Vector2Int, Vector3Int>> _tileDict = new();
        
        public static HeatmapDisplay Instance;

        private float _halfChunkSize;
        
        private void Awake()
        {
            Instance = this;
            UIEvents.UIMap.OnOpenHeatmap += OnChangeMap;
            UIEvents.UIMap.OnUpdateHeatmapChunks += UpdateHeatMapChunks;
            UIEvents.UIMap.OnUpdateMultipleHeatmapChunks += UpdateMultipleHeatMapChunks;

            if (SO_fileLoc.isBuild)
                GameEvents.Lifecycle.OnTileManagerFinishedInitializing += LoadTextures;
        }

        private void OnDestroy()
        {
            UIEvents.UIMap.OnOpenHeatmap -= OnChangeMap;
            UIEvents.UIMap.OnUpdateHeatmapChunks -= UpdateHeatMapChunks;
            UIEvents.UIMap.OnUpdateMultipleHeatmapChunks -= UpdateMultipleHeatMapChunks;
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
            _halfChunkSize = _chunkSize / 2;
            _tileManager = TileManager.Instance;
            
            var _worldPosition = new Vector3(0, 0, 0);
            var tilemapGen = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
            var heatmaps = heatmapGenerator.GenerateAllHeatmaps(tilemapGen);

            _maps[MapDisplay.MapOverlay.Terrain] = MapExtractor.Instance.GetTerrainTextures();
            
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    _tileDict[new Vector2(x, y)] = new Dictionary<Vector2Int, Vector3Int>();
                    foreach (var overlay in _overlayValueByTile.Keys)
                    {
                        var chunk = new Vector2(x, y);
                        var tex = heatmaps[chunk][overlay];
                        _maps[overlay].Add(chunk, tex);
                        
                        foreach (var coord in VectorUtils.GridCoordinates(_chunkSize, _chunkSize))
                        {
                            _worldPosition.x = (x * (_chunkSize - 1)) + (coord.x - _halfChunkSize) + 1;
                            _worldPosition.z = -(y * (_chunkSize - 1)) + (coord.y - _halfChunkSize);
                            _tileDict[new Vector2(x, y)][coord] = _tileManager.getTileDataKeyByWorldCoords(_worldPosition);
                        }
                    }
                }
            }
        }
        
        private void LoadTexturesFromDrive()
        {
            _halfChunkSize = _chunkSize / 2;
            _tileManager = TileManager.Instance;
            
            var _worldPosition = new Vector3(0, 0, 0);
            
            _maps[MapDisplay.MapOverlay.Terrain] = MapExtractor.Instance.GetTerrainTextures();
            
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    _tileDict[new Vector2(x, y)] = new Dictionary<Vector2Int, Vector3Int>();
                    
                    foreach (var overlay in _overlayValueByTile.Keys)
                    {
                        var bytes = File.ReadAllBytes($"{Application.dataPath}/2D/GeneratedHeatmaps/{overlay}_{x}_{y}.jpg");
                        var tex = new Texture2D(_chunkSize, _chunkSize, TextureFormat.ARGB32, false);
                        tex.LoadImage(bytes);
                        _maps[overlay].Add(new Vector2(x, y), tex);
                        
                        foreach (var coord in VectorUtils.GridCoordinates(_chunkSize, _chunkSize))
                        {
                            _worldPosition.x = (x * (_chunkSize - 1)) + (coord.x - _halfChunkSize) + 1;
                            _worldPosition.z = -(y * (_chunkSize - 1)) + (coord.y - _halfChunkSize);
                            _tileDict[new Vector2(x, y)][coord] = _tileManager.getTileDataKeyByWorldCoords(_worldPosition);
                        }
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
        
        private async void UpdateHeatMapChunks(List<Vector2> chunkPos, MapDisplay.MapOverlay overlay)
        {
            
            foreach (var pos in chunkPos)
            {
                var colorMap = await OnSingleHeatmapOnChunk(pos, overlay);
                _maps[overlay][pos] = TextureGenerator.TextureFromColorMap(colorMap, _chunkSize);
            }
            
            if (_currentMapOverlay == overlay)
                mapDisplay.ReplaceTexture(_maps[overlay]);
        }
        
        private async void UpdateMultipleHeatMapChunks(List<Vector2> chunkPos, MapDisplay.MapOverlay[] overlays)
        {
            var tasks = new List<Task>();
            
            foreach (var overlay in overlays)
            {
                foreach (var pos in chunkPos)
                {
                    tasks.Add(ProcessSingleChunk(pos, overlay));
                }
            }
            
            await Task.WhenAll(tasks);
            
            if (overlays.Contains(_currentMapOverlay))
                mapDisplay.ReplaceTexture(_maps[_currentMapOverlay]);
        }

        private async Task ProcessSingleChunk(Vector2 pos, MapDisplay.MapOverlay overlay)
        {
            var colorMap = await OnSingleHeatmapOnChunk(pos, overlay);
            _maps[overlay][pos] = TextureGenerator.TextureFromColorMap(colorMap, _chunkSize);
        }
        
        private async Task<Color[]> OnSingleHeatmapOnChunk(Vector2 chunk, MapDisplay.MapOverlay overlay)
        {
            var colorMap = new Color[_chunkSize * _chunkSize];
            var tileChunk = _tileDict[chunk];
            
            var heatmapOverlay = _heatmapDict[overlay];
            var min = heatmapOverlay.min;
            var max = heatmapOverlay.max;
            var gradient = heatmapOverlay.gradient;
            var heatPointNull = HeatToColor(gradient, min, min, max);
            
            return await Task.Run(() =>
            {
                foreach (var coord in VectorUtils.GridCoordinates(_chunkSize, _chunkSize))
                {
                    var tileGridCoords = tileChunk[coord];
                    var tile = _tileManager.getTileDataByGridCoords(tileGridCoords);

                    var colorMapCoordinate = (_chunkSize - 1 - coord.y) * _chunkSize + coord.x;

                    if (tile == null)
                    {
                        colorMap[colorMapCoordinate] = heatPointNull;
                        continue;
                    }

                    var tileValue = _overlayValueByTile[overlay](tile);

                    var heatPoint = HeatToColor(gradient, tileValue, min, max);

                    colorMap[colorMapCoordinate] = heatPoint;
                }

                return colorMap;
            }).ConfigureAwait(false);;
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