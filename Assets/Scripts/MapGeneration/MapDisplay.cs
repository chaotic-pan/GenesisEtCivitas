// MADE BY GENESIS

using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private List<Heatmap> heatmaps;
        
        public GameObject meshRendererPrefab;
        private const int size = 239;
        private int scale = 1;

        private Dictionary<Vector2, GameObject> _meshInstances = new();
        
        public void DrawMeshes(Dictionary<Vector2,MeshData> meshDataDict, Dictionary<Vector2,Texture2D> textures)
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
            Climate,
            WaterValue
        }
    }
}