// MADE BY GENESIS

using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class MapDisplay : MonoBehaviour
    {
        public GameObject meshRendererPrefab;
        private const int size = 239;
        private int scale = 1;

        private Dictionary<Vector2, GameObject> _meshInstances = new();
        
        public static MapDisplay Instance;
        private void Awake()
        {
            Instance = this;
            FillMeshInstancesDictionary();
            LoadTerrainTexturesIntoMapsDictionary();
        }
    
        private void FillMeshInstancesDictionary()
        {
            _meshInstances.Clear();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                
                var chunkX = Mathf.RoundToInt(child.position.x / (scale * size));
                var chunkY = Mathf.RoundToInt(-child.position.z / (scale * size));
                var chunkCoordinate = new Vector2(chunkX, chunkY);
            
                _meshInstances[chunkCoordinate] = child.gameObject;
            }
        }

        private void LoadTerrainTexturesIntoMapsDictionary()
        {
            
        }
        
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