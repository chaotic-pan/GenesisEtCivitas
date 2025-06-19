using System;
using System.Collections.Generic;
using System.Linq;
using Terrain;
using UI;
using UnityEngine;
using Utilities;

namespace MapGeneration
{
    public class HeatmapDisplay : MonoBehaviour
    {
        [SerializeField] private List<Heatmap> heatmaps;
        [SerializeField] private MapDisplay mapDisplay;
        
        public readonly Dictionary<MapDisplay.MapOverlay, Dictionary<Vector2, Texture2D>> Maps = new();
        
        private Dictionary<MapDisplay.MapOverlay, Heatmap> _heatmapDict = new();
        private TileManager _tileManager;
        private readonly int _chunkSize = 240;
        private MapDisplay.MapOverlay _currentMapOverlay;
        
        private readonly Dictionary<MapDisplay.MapOverlay, Func<TileData, float>> _overlayValueByTile = new()
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
        
        private void Awake()
        {
            UIEvents.UIMap.OnOpenHeatmap += OnChangeMap;
            UIEvents.UIMap.OnUpdateHeatmapChunks += UpdateHeatMapChunks;
            
            _heatmapDict = heatmaps.ToDictionary(h => h.overlay, h => h);
        }

        private void UpdateHeatMapChunks(List<Vector2> chunkPos, MapDisplay.MapOverlay overlay)
        {
            foreach (var pos in chunkPos)
            {
                OnSingleHeatmapOnChunk(pos, overlay);
            }
            
            if (_currentMapOverlay == overlay)
                mapDisplay.ReplaceTexture(Maps[overlay]);
        }

        public void Initialize()
        {
            _tileManager = TileManager.Instance;
            
            InitializeMaps();
            GenerateAllHeatmaps();
        }
        
         private void InitializeMaps()
        {
            foreach (var mapOverlay in _overlayValueByTile.Keys)
            {
                Maps[mapOverlay] = new Dictionary<Vector2, Texture2D>();
            } 
        }

        private void GenerateAllHeatmaps()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    UpdateHeatmapOnChunk(new Vector2(x, y));
                }
            }
        }

        private void UpdateHeatmapOnChunk(Vector2 chunk)
        {
            var worldPosition = new Vector3(0, 0, 0);
            var halfChunkSize = _chunkSize / 2;

            var colorMaps = new Dictionary<MapDisplay.MapOverlay, Color[]>();

            foreach (var overlay in _overlayValueByTile.Keys)
            {
                colorMaps[overlay] = new Color[_chunkSize * _chunkSize];
            }
            
            foreach (var coord in VectorUtils.GridCoordinates(_chunkSize, _chunkSize))
            {
                    worldPosition.x = (chunk.x * (_chunkSize - 1)) + (coord.x - halfChunkSize) + 1;
                    worldPosition.z = -(chunk.y * (_chunkSize - 1)) + (coord.y - halfChunkSize);
                    
                    var tile = _tileManager.getTileDataByWorldCoords(worldPosition);
                    
                    var colorMapCoordinate = (_chunkSize - 1 - coord.y) * _chunkSize + coord.x;

                    if (tile == null)
                    {
                        foreach (var overlay in _overlayValueByTile.Keys)
                        {
                            var heatPointNull = HeatToColor(_heatmapDict[overlay].gradient,
                                _heatmapDict[overlay].min, _heatmapDict[overlay].min,
                                _heatmapDict[overlay].max);

                            colorMaps[overlay][colorMapCoordinate] = heatPointNull;
                        }
                        
                        continue;
                    }
                    
                    foreach (var overlay in _overlayValueByTile.Keys)
                    {
                        var tileValue = _overlayValueByTile[overlay](tile);
                        
                        var heatPoint = HeatToColor(_heatmapDict[overlay].gradient,
                            tileValue, _heatmapDict[overlay].min,
                            _heatmapDict[overlay].max);

                        colorMaps[overlay][colorMapCoordinate] = heatPoint;
                    }
            }
            
            foreach (var overlay in _overlayValueByTile.Keys)
            {
                var tileValue = colorMaps[overlay];
                Maps[overlay][chunk] = TextureGenerator.TextureFromColorMap(tileValue, _chunkSize);
            }
            
        }

        private void OnSingleHeatmapOnChunk(Vector2 chunk, MapDisplay.MapOverlay overlay)
        {
            var worldPosition = new Vector3(0, 0, 0);
            var halfChunkSize = _chunkSize / 2;
            
            var colorMap = new Color[_chunkSize * _chunkSize];
            
            foreach (var coord in VectorUtils.GridCoordinates(_chunkSize, _chunkSize))
            {
                    worldPosition.x = (chunk.x * (_chunkSize - 1)) + (coord.x - halfChunkSize) + 1;
                    worldPosition.z = -(chunk.y * (_chunkSize - 1)) + (coord.y - halfChunkSize);
                    
                    var tile = _tileManager.getTileDataByWorldCoords(worldPosition);
                    
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
            
            Maps[overlay][chunk] = TextureGenerator.TextureFromColorMap(colorMap, _chunkSize);
        }
        
        

        private void OnChangeMap(MapDisplay.MapOverlay mapOverlay)
        {
            mapDisplay.ReplaceTexture(Maps[mapOverlay]);
            _currentMapOverlay = mapOverlay;
        }

        private Color HeatToColor(Gradient gradient, float value, float min, float max)
        {
            var normalized = (value - min) / (max - min);
            return gradient.Evaluate(normalized);
        }
    }
}