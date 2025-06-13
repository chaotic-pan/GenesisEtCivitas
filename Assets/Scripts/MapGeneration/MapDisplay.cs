// MADE BY GENESIS

using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using Utilities;

namespace Terrain
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private List<Heatmap> heatmaps;
        
        public GameObject meshRendererPrefab;
        private const int size = 239;
        private int scale = 1;

        private Dictionary<Vector2, GameObject> _meshInstances = new();
        
        public readonly Dictionary<MapOverlay, Dictionary<Vector2, Texture2D>> Maps = new();
        private Dictionary<MapOverlay, Heatmap> _heatmapDict = new();
        

        private void Awake()
        {
            UIEvents.UIMap.OnOpenTest += OnChangeMap;
            _heatmapDict = heatmaps.ToDictionary(h => h.overlay, h => h);
        }

        private void Start()
        {
            var mapExtractor = MapExtractor.Instance;
            
            var travelcostMap = mapExtractor.GetColorData(MapExtractor.Instance.travelcost, _heatmapDict[MapOverlay.Travelcost]);
            Maps[MapOverlay.Travelcost] = TextureGenerator.TextureFromColorMaps(travelcostMap, 240);
            
            var fertility = mapExtractor.GetColorData(MapExtractor.Instance.fertility, _heatmapDict[MapOverlay.Fertility]);
            Maps[MapOverlay.Fertility] = TextureGenerator.TextureFromColorMaps(fertility, 240);

            var firmness = mapExtractor.GetColorData(MapExtractor.Instance.firmness, _heatmapDict[MapOverlay.Firmness]);
            Maps[MapOverlay.Firmness] = TextureGenerator.TextureFromColorMaps(firmness, 240);

            var ore = mapExtractor.GetColorData(MapExtractor.Instance.ore, _heatmapDict[MapOverlay.Ore]);
            Maps[MapOverlay.Ore] = TextureGenerator.TextureFromColorMaps(ore, 240);

            var vegetation = mapExtractor.GetColorData(MapExtractor.Instance.vegetation, _heatmapDict[MapOverlay.Vegetation]);
            Maps[MapOverlay.Vegetation] = TextureGenerator.TextureFromColorMaps(vegetation, 240);

            var animalPopulation = mapExtractor.GetColorData(MapExtractor.Instance.animalPopulation, _heatmapDict[MapOverlay.AnimalPopulation]);
            Maps[MapOverlay.AnimalPopulation] = TextureGenerator.TextureFromColorMaps(animalPopulation, 240);

            var animalHostility = mapExtractor.GetColorData(MapExtractor.Instance.animalHostility, _heatmapDict[MapOverlay.AnimalHostility]);
            Maps[MapOverlay.AnimalHostility] = TextureGenerator.TextureFromColorMaps(animalHostility, 240);

            var climate = mapExtractor.GetColorData(MapExtractor.Instance.climate, _heatmapDict[MapOverlay.Climate]);
            Maps[MapOverlay.Climate] = TextureGenerator.TextureFromColorMaps(climate, 240);

        }

        private void OnChangeMap(MapOverlay mapOverlay)
        {
            ReplaceTexture(Maps[mapOverlay]);
        }

        public void DrawMeshes(Dictionary<Vector2,MeshData> meshDataDict, Dictionary<Vector2,Texture2D> textures)
        {
            foreach (var chunkCoordinate in meshDataDict.Keys)
            {
                var newMesh = Instantiate(meshRendererPrefab, 
                    new Vector3(scale * size * chunkCoordinate.x, 0, -scale * size * chunkCoordinate.y), 
                    Quaternion.identity, 
                    transform);
                newMesh.transform.localScale = new Vector3(scale,scale,scale);
                
                var meshData = meshDataDict[chunkCoordinate];
                newMesh.GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
                var meshRenderer = newMesh.GetComponent<MeshRenderer>();
                var material = new Material(meshRenderer.sharedMaterial)
                {
                    mainTexture = textures[chunkCoordinate]
                };
                meshRenderer.sharedMaterial = material;
            }
        }
        
        public void DrawMeshes_(Dictionary<Vector2,MeshData> meshDataDict, Dictionary<Vector2,Texture2D> textures)
        {
            foreach (var chunkCoordinate in meshDataDict.Keys)
            {
                _meshInstances[chunkCoordinate] = Instantiate(meshRendererPrefab, 
                    new Vector3(scale * size * chunkCoordinate.x, 0, -scale * size * chunkCoordinate.y), 
                    Quaternion.identity, 
                    transform);

                var newMesh = _meshInstances[chunkCoordinate];
                newMesh.transform.localScale = new Vector3(scale,scale,scale);
                
                var meshData = meshDataDict[chunkCoordinate];
                newMesh.GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
                var meshRenderer = newMesh.GetComponent<MeshRenderer>();
                var material = new Material(meshRenderer.sharedMaterial)
                {
                    mainTexture = textures[chunkCoordinate]
                };
                meshRenderer.sharedMaterial = material;
            }
        }

        public void ReplaceTexture(Dictionary<Vector2, Texture2D> textures)
        {
            foreach (var chunkCoordinate in textures.Keys)
            {
                var mesh = _meshInstances[chunkCoordinate];
                var meshRenderer = mesh.GetComponent<MeshRenderer>();
            
                var material = new Material(meshRenderer.sharedMaterial)
                {
                    mainTexture = textures[chunkCoordinate]
                };
                meshRenderer.sharedMaterial = material;
            }
        }
        
        public Color HeatToColor(Gradient gradient, float value, float min, float max)
        {
            var normalized = (value - min) / (max - min);
            return gradient.Evaluate(normalized);
        }

        public enum MapOverlay
        {
            Terrain,
            Travelcost,
            Fertility,
            Firmness,
            Ore,
            Vegetation,
            AnimalPopulation,
            AnimalHostility,
            Climate
        }
    }
}